parser grammar AplusParser;

options {
	language = CSharp2;
	tokenVocab=AplusLexer;
	backtrack = true;
	
}
	
@parser::namespace { AplusCore.Compiler.Grammar }

@header {
using System.Collections.Generic;
}

@members {
    private Dictionary<string, List<AST.Identifier>> localAssignments = null;
	private Dictionary<string, List<AST.Identifier>> globalAssignments = null;


	/// <summary>
	/// Describes if the current parsing state is inside a function.
	/// </summary>
	private bool isfunction = false;
	private AST.Node function;

	/// <summary>
	/// Describes if the current parsing state is inside a function.
	/// </summary>
	private bool isdependency = false;
	private HashSet<AST.Identifier> variableAccessing = null;

	public AST.Node tree;

	public bool ParseOk { get { return NumberOfSyntaxErrors == 0; } }

	public bool Parse() {
		script();
		return ParseOk;
	}

    public override void ReportError(RecognitionException exception)
    {
    	// Add error report here
    	base.ReportError(exception);
		
		if (exception is Antlr.Runtime.NoViableAltException)
        {
            // Throw an error that we can't continue the execution/parsing of this input
            throw new ParseException("No Viable Alternate", false, exception);
        }
		else if (exception is Antlr.Runtime.UnwantedTokenException)
        {
			UnwantedTokenException ex = (UnwantedTokenException)exception;
			// check if we can continue
			bool canContinue = (ex.Token.Text == "{"); // TODO: change this a constant
            // Throw an error that we can't continue the execution/parsing of this input
            throw new ParseException("Unwanted token found:" + ex.Token.Text, canContinue, ex);
        }
		// Throw an error, maybe we can continue the execution?
    	throw new ParseException(exception.Message, true, exception);
    }

	private void SetupUserDefFunction()
	{
		this.localAssignments = new Dictionary<string, List<AST.Identifier>>();
		this.globalAssignments = new Dictionary<string, List<AST.Identifier>>();
		this.isfunction = true;
		this.function = null;
	}

	private void TearDownUserDefFunction()
	{
		this.localAssignments = null;
		this.globalAssignments = null;
		this.isfunction = false;
		this.function = null;
	}

	private AST.Node BuildMonadic(AST.Token symbol, AST.Node argument)
	{
	    AST.Node node;
		
		if(symbol.Type == Tokens.DO)
		{
			node = AST.Node.MonadicDo(argument);
		}
		else if(argument is AST.ExpressionList)
		{
			node = AST.Node.BuiltinInvoke(symbol, (AST.ExpressionList)argument);
		}
		else
		{
			node = AST.Node.MonadicFunction(symbol, argument); 
		}

		return node;
	}

	private void AssignmentPreprocessor(AST.Node lhs)
	{
		if(!(lhs is AST.Identifier))
		{
			return; // only need to handle simple assignments
		}
		if(!this.isfunction)
		{
			return;
		}
		AST.Identifier target = (AST.Identifier)lhs;

		if(target.Type != AST.IdentifierType.UnQualifiedName)
		{
			return; 
		}

		if(target.IsEnclosed)
		{
			if (this.localAssignments.ContainsKey(target.Name))
			{
				// Found the variable already used in a local assignment
				target.IsEnclosed = false;
				this.localAssignments[target.Name].Add(target);
			}
			else 
			{
				if (!this.globalAssignments.ContainsKey(target.Name))
				{
					// variable did not exists currently as global assignment
					this.globalAssignments[target.Name] = new List<AST.Identifier>();
				}
				// add the target as a global assignment target
				this.globalAssignments[target.Name].Add(target);
			}
		}
		else
		{
			if (!this.localAssignments.ContainsKey(target.Name))
			{
				this.localAssignments[target.Name] = new List<AST.Identifier>();
			}

			if (this.globalAssignments.ContainsKey(target.Name))
			{
				// found the same variable as a global assignment target
				//  move it to the local assignments
				foreach (AST.Identifier item in this.globalAssignments[target.Name])
				{
					item.IsEnclosed = false;
					this.localAssignments[target.Name].Add(item);
				}

				// remove from the global assignments' list
				this.globalAssignments.Remove(target.Name);
			}

			this.localAssignments[target.Name].Add(target);
		}

	}

	private AST.Node BuildDyadic(AST.Token symbol, AST.Node lhs, AST.Node rhs)
	{
		AST.Node node;

		if(lhs is AST.BuiltInFunction)
		{
			/* This will allow the following construct:
				((-)) * 5
			*/
			node = BuildMonadic(((AST.BuiltInFunction)lhs).Function, BuildMonadic(symbol, rhs));
		}
		else if(lhs is AST.BuiltInOperator)
		{
			/* This will allow the following construct:
				((-each)) * 5
			*/
			
			AST.Operator op = ((AST.BuiltInOperator)lhs).Operator;
			op.RightArgument = BuildMonadic(symbol, rhs);

			node = op;
		}
		else
		{
			switch(symbol.Type)
			{
				case Tokens.DO:
					node = AST.Node.DyadicDo(lhs, rhs);
					break;

				case Tokens.RESULT: // Tokens.Assign
					AssignmentPreprocessor(lhs);
					node = AST.Node.Assign(lhs, rhs);
					break;

				default:
					if(rhs is AST.ExpressionList)
					{
						throw new ParseException("Incorrect call format", false);
					}

					node = AST.Node.DyadicFunction(symbol, lhs, rhs);
					break;
			}
		}

		return node;
	}

}

script // returns [AST.Node tree]
	:	statements EOF 			{ tree = $statements.node; }
	;
	
statements returns [AST.ExpressionList node]
	@init { node = AST.Node.ExpressionList(); } 
	:	(	items=statement						{ node.AddLast($items.node); }
			NewLine
		 )*
		last=statement							{ node.AddLast($last.node); }
	;
	
statement returns [AST.Node node]
	:	systemCommand						{ node = $systemCommand.node; }
	|	dependencyDefinition				{ node = $dependencyDefinition.node; }
	|	userDefinedFunction					{ node = $userDefinedFunction.node; }
	|	expressionList						{ node = $expressionList.node; }
	;
	
systemCommand returns [AST.SystemCommand node]
	:	SystemCommand				{ node = AST.Node.SystemCommand($SystemCommand.Text); }
		(
			SystemCommandArgument	{ node.Argument = $SystemCommandArgument.Text; }
		)?
	;
	
dependencyDefinition returns [AST.Node node]
	@init{	this.variableAccessing = new HashSet<AST.Identifier>();
			this.isdependency = false; 
			SetupUserDefFunction(); 
		}
	@after{ this.variableAccessing = null;
			this.isdependency = false;
			TearDownUserDefFunction();
		}
	:	variableName { this.isdependency = true; }	Colon  functionBody
			{ 
				AST.Dependency.UpdateDependantSet(this.localAssignments, this.globalAssignments, this.variableAccessing);
				$node = AST.Node.Dependency($variableName.node, $functionBody.node, $text, this.variableAccessing);
			}
	;

userDefinedFunction  returns [AST.Node node]
	@init { SetupUserDefFunction(); }
	@after { TearDownUserDefFunction(); }
	:	variableName										{ this.function = $variableName.node; }
		expressionGroup Colon functionBody
			{ $node = AST.Node.UserDefFunction($variableName.node, $expressionGroup.node, $functionBody.node, $text); }
	|	arg1=variableName
		name=variableName									{ this.function = $name.node; } 
		arg2=variableName Colon functionBody
			{ $node = AST.Node.UserDefFunction($name.node, 
						AST.Node.ExpressionList($arg1.node, $arg2.node), $functionBody.node, $text); 
			}
	|	name=variableName									{ this.function = $name.node; }
		arg1=variableName Colon functionBody
		{ $node = AST.Node.UserDefFunction($name.node, 
						AST.Node.ExpressionList($arg1.node), $functionBody.node, $text); 
			}
	;
	
functionBody returns [AST.Node node]
	:	expression					{ node = $expression.node; }
	|	NewLine expression					{ node = $expression.node; }
	;


expressionList returns [AST.ExpressionList node]
	@init { AST.ExpressionList list = AST.Node.ExpressionList(); }
	:	fist=topExpressionNull		{ list.AddLast($fist.node); }
		(SemiColon
		 item=topExpressionNull		{ list.AddLast($item.node); }
		)*
		{ node = list; }
	;

topExpressionNull returns [AST.Node node]
	:	expression							{ node = $expression.node; }
	|	operatorSymbol						{ node = AST.Node.BuiltInOperator($operatorSymbol.node); }
	|	functionSymbol						{ node = AST.Node.BuiltInFunction($functionSymbol.token); }
	|	/* Null */							{ node = AST.Node.NullConstant(); }
	;

expressionGroup returns [AST.ExpressionList node]
	:	LSB RSB								{ node = AST.Node.ExpressionList(); }
	|	LSB expressionList RSB				{ node = $expressionList.node; }
	;

expression returns [AST.Node node]
	:	controlStatements								{ node = $controlStatements.node; }
	|	lhs=dyadicLeftArg func=functionSelector rhs=expression
			{
				if($func.node is AST.Token)
				{
					node = BuildDyadic((AST.Token)$func.node, lhs, rhs);
				}
				else if($func.node is AST.Operator)
				{
					AST.Operator oper = (AST.Operator)$func.node;
					oper.RightArgument = $rhs.node;
					oper.LeftArgument = $lhs.node;
					node = oper;
				}
				else
				{
					throw new ParseException("Should Not reach this point!", false);
				}
			}
	|	func=functionSelector arg=expression
			{
				if($func.node is AST.Token)
				{
					node = BuildMonadic((AST.Token)$func.node, $arg.node);
				}
				else if($func.node is AST.Operator)
				{
					AST.Operator oper = (AST.Operator)$func.node;
					if($arg.node is AST.ExpressionList)
					{
						node = AST.Node.BuiltinOpInvoke(oper, (AST.ExpressionList)$arg.node);
					}
					else
					{
						oper.RightArgument = $arg.node;
						node = oper;
					}
				}
				else
				{
					throw new ParseException("Should Not reach this point!", false);
				}
			}
	|	simpleExpression				{ node = $simpleExpression.node; }
	;

functionSelector returns [AST.Node node]
	:	operatorSymbol							{ node = $operatorSymbol.node; }
	|	functionSymbol							{ node = $functionSymbol.token; }
	;

dyadicLeftArg returns [AST.Node node]
	:	operatorSymbol expressionGroup			{ node = AST.Node.BuiltinOpInvoke($operatorSymbol.node, $expressionGroup.node); }
	|	functionSymbol expressionGroup			{ node = AST.Node.BuiltinInvoke($functionSymbol.token, (AST.ExpressionList)$expressionGroup.node); }
	|	simpleExpression						{ node = $simpleExpression.node; }
	;

operatorSymbol returns [AST.Operator node]
	@init { AST.Node func = null; }
	:
		opArg					{ func = $opArg.node; }
		(
			opTail				{ 
									if(func is AST.Operator)
									{
										// Found an operator, wrap it as a Built-in operator
										$opTail.node.Function = AST.Node.BuiltInOperator(func);
									}
									else
									{
										$opTail.node.Function = func; 
									}
									node = $opTail.node;
									func = node;
								}
		)+
	|	LP op=operatorSymbol RP				{ node = $op.node; }
	;

opArg returns [AST.Node node]
	:	functionSymbol						{ node = $functionSymbol.token; }
	|	termExpression						{ node = $termExpression.node; }
	;

opTail returns [AST.Operator node]
	:	Each								{ node = AST.Node.EachOperator(new AST.Token(Tokens.EACH, $Each.Text)); }
	|	Rank condition						{ node = AST.Node.RankOperator(new AST.Token(Tokens.RANK, $Rank.Text));
											  ((AST.RankOperator)node).Condition = $condition.node; }
	;

functionSymbol returns [AST.Token token]
	:	LP sym=functionSymbol RP			{ token = $sym.token; }
	// Monadic Symbols
	|	t=AbsoluteValue						{ token = new AST.Token(Tokens.ABSOLUTEVALUE, $t.Text); }
	|	t=Ceiling							{ token = new AST.Token(Tokens.CEILING, $t.Text); }
	|	t=Count								{ token = new AST.Token(Tokens.COUNT, $t.Text); }
	|	t=DefaultFormat						{ token = new AST.Token(Tokens.DEFAULTFORMAT, $t.Text); }
	|	t=Depth								{ token = new AST.Token(Tokens.DEPTH, $t.text); }
	|	t=Disclose							{ token = new AST.Token(Tokens.DISCLOSE, $t.Text); }
	|	t=Enclose							{ token = new AST.Token(Tokens.ENCLOSE, $t.Text); }
	|	t=Execute							{ token = new AST.Token(Tokens.EXECUTE, $t.Text); }
	|	t=Exponential						{ token = new AST.Token(Tokens.EXPONENTIAL, $t.Text); }
	|	t=Floor								{ token = new AST.Token(Tokens.FLOOR, $t.Text); }
	|	t=GradeDown							{ token = new AST.Token(Tokens.GRADEDOWN, $t.Text); }
	|	t=GradeUp							{ token = new AST.Token(Tokens.GRADEUP, $t.Text); }
	|	t=Identity							{ token = new AST.Token(Tokens.IDENTITY, $t.Text); }
	|	t=Interval							{ token = new AST.Token(Tokens.INTERVAL, $t.Text); }
	|	t=ItemRavel							{ token = new AST.Token(Tokens.ITEMRAVEL, $t.Text); }
	|	t=MapIn								{ token = new AST.Token(Tokens.MAPIN, $t.Text); }
	|	t=NaturalLog						{ token = new AST.Token(Tokens.NATURALLOG, $t.Text); }
	|	t=Negate							{ token = new AST.Token(Tokens.NEGATE, $t.Text); }
	|	t=Not								{ token = new AST.Token(Tokens.NOT, $t.Text); }
	|	t=Null								{ token = new AST.Token(Tokens.NULL, $t.Text); }
	|	t=Pack								{ token = new AST.Token(Tokens.PACK, $t.Text); }
	|	t=PartitionCount					{ token = new AST.Token(Tokens.PARTITIONCOUNT, $t.Text); }
	|	t=PiTimes							{ token = new AST.Token(Tokens.PITIMES, $t.Text); }
	|	t=Print								{ token = new AST.Token(Tokens.PRINT, $t.Text); }
	|	t=Rake								{ token = new AST.Token(Tokens.RAKE, $t.Text); }
	|	t=Raze                              { token = new AST.Token(Tokens.RAZE, $t.Text); } 
	|	t=Ravel								{ token = new AST.Token(Tokens.RAVEL, $t.Text); }
	|	t=Reciprocal						{ token = new AST.Token(Tokens.RECIPROCAL, $t.Text); }
	|	t=Result							{ token = new AST.Token(Tokens.RESULT, $t.Text); }
	|	t=Reverse							{ token = new AST.Token(Tokens.REVERSE, $t.Text); }
	|	t=Roll								{ token = new AST.Token(Tokens.ROLL, $t.Text); }
	|	t=Right								{ token = new AST.Token(Tokens.RIGHT, $t.Text); }
	|	t=SeparateSymbols					{ token = new AST.Token(Tokens.SEPARATESYMBOLS, $t.Text); }
	|	t=Shape								{ token = new AST.Token(Tokens.SHAPE, $t.Text); }
	|	t=Sign								{ token = new AST.Token(Tokens.SIGN, $t.Text); }
	|	t=Signal							{ token = new AST.Token(Tokens.SIGNAL, $t.Text); }
	|	t=Stop								{ token = new AST.Token(Tokens.STOP, $t.Text); }
	|	t=Transpose							{ token = new AST.Token(Tokens.TRANSPOSE, $t.Text); }
	|	t=Unpack							{ token = new AST.Token(Tokens.UNPACK, $t.Text); }
	|	t=Value								{ token = new AST.Token(Tokens.VALUE, $t.text); }

	|	t=Or								{ token = new AST.Token(Tokens.TYPE, $t.Text); }

	// Dyadic Products:
	// inner products
	|	t=IPAddMultiply						{ token = new AST.Token(Tokens.IPADDMULTIPLY, $t.Text); }
	|	t=IPMaxAdd							{ token = new AST.Token(Tokens.IPMAXADD, $t.Text); }
	|	t=IPMinAdd							{ token = new AST.Token(Tokens.IPMINADD, $t.Text); }
	// outer products	
	|	t=OPAdd								{ token = new AST.Token(Tokens.OPADD, $t.Text); }
	|	t=OPDivide							{ token = new AST.Token(Tokens.OPDIVIDE, $t.Text); }
	|	t=OPEqual							{ token = new AST.Token(Tokens.OPEQUAL, $t.Text); }
	|	t=OPGT								{ token = new AST.Token(Tokens.OPGT, $t.Text); }
	|	t=OPGTE								{ token = new AST.Token(Tokens.OPGTE, $t.Text); }
	|	t=OPLT								{ token = new AST.Token(Tokens.OPLT, $t.Text); }
	|	t=OPLTE								{ token = new AST.Token(Tokens.OPLTE, $t.Text); }
	|	t=OPMax								{ token = new AST.Token(Tokens.OPMAX, $t.Text); }
	|	t=OPMin								{ token = new AST.Token(Tokens.OPMIN, $t.Text); }
	|	t=OPMultiply						{ token = new AST.Token(Tokens.OPMULTIPLY, $t.Text); }
	|	t=OPNotEqual						{ token = new AST.Token(Tokens.OPNOTEQUAL, $t.Text); }
	|	t=OPResidue							{ token = new AST.Token(Tokens.OPRESIDUE, $t.Text); }
	|	t=OPSubstract						{ token = new AST.Token(Tokens.OPSUBSTRACT, $t.Text); }
	|	t=OPPower							{ token = new AST.Token(Tokens.OPPOWER, $t.Text); }

	// Monadic Reduces:
	|	t=RAdd								{ token = new AST.Token(Tokens.RADD, $t.Text); }
	|	t=RAnd								{ token = new AST.Token(Tokens.RAND, $t.Text); }
	|	t=RMax								{ token = new AST.Token(Tokens.RMAX, $t.Text); }
	|	t=RMin								{ token = new AST.Token(Tokens.RMIN, $t.Text); }
	|	t=RMultiply							{ token = new AST.Token(Tokens.RMULTIPLY, $t.Text); }
	|	t=ROr								{ token = new AST.Token(Tokens.ROR, $t.Text); }
	// Monadic Scans:
	|	t=SAdd								{ token = new AST.Token(Tokens.SADD, $t.Text); }
	|	t=SAnd								{ token = new AST.Token(Tokens.SAND, $t.Text); }
	|	t=SMax								{ token = new AST.Token(Tokens.SMAX, $t.Text); }
	|	t=SMin								{ token = new AST.Token(Tokens.SMIN, $t.Text); }
	|	t=SMultiply							{ token = new AST.Token(Tokens.SMULTIPLY, $t.Text); }
	|	t=SOr								{ token = new AST.Token(Tokens.SOR, $t.Text); }

	// only Dyadic Symbols
	|	t=Equal								{ token = new AST.Token(Tokens.EQUAL, $t.Text); }
	|	t=Expand							{ token = new AST.Token(Tokens.EXPAND, $t.Text); }
	|	t=GTE								{ token = new AST.Token(Tokens.GTE, $t.Text); }
	|	t=LTE								{ token = new AST.Token(Tokens.LTE, $t.Text); }
	|	t=NotEqual							{ token = new AST.Token(Tokens.NOTEQUAL, $t.Text); }
	|	t=Replicate							{ token = new AST.Token(Tokens.REPLICATE, $t.Text); }

	// bitwise operators
	|	t=BWNot								 { token = new AST.Token(Tokens.BWNOT, $t.Text); }
	|	t=BWAnd								 { token = new AST.Token(Tokens.BWAND, $t.Text); }
	|	t=BWOr								 { token = new AST.Token(Tokens.BWOR, $t.Text); }
	|	t=BWLT								 { token = new AST.Token(Tokens.BWLESS, $t.Text); }
	|	t=BWLTE								 { token = new AST.Token(Tokens.BWLESSEQUAL, $t.Text); }
	|	t=BWGT								 { token = new AST.Token(Tokens.BWGREATER, $t.Text); }
	|	t=BWGTE								 { token = new AST.Token(Tokens.BWGREATEREQUAL, $t.Text); }
	|	t=BWEqual							 { token = new AST.Token(Tokens.BWEQUAL, $t.Text); }
	|	t=BWNotEqual						 { token = new AST.Token(Tokens.BWNOTEQUAL, $t.Text); }

	//	Dyadic and Monadic do
	|  Do									{ token = AST.Node.Token(Tokens.DO); }
	;
	
	

controlStatements returns [AST.Node node]
	:	ifStatement							{ node = $ifStatement.node; }
	|	caseStatement						{ node = $caseStatement.node; }
	|	whileStatement						{ node = $whileStatement.node; }
	;

condition returns [AST.Node node]
	:	termExpression					{ node = $termExpression.node; }
	;

ifStatement returns [AST.If node]
	:	If condition truecase=topExpressionNull		{ node = AST.Node.If($condition.node, $truecase.node); }
		(Else
		 falsecase=topExpressionNull				{ node.AddFalseCase($falsecase.node); }
		)?
	;
	
caseStatement returns [AST.Case node]
	:	Case condition expressionGroup			{ node = AST.Node.Case($condition.node, $expressionGroup.node); }
	;
	
whileStatement returns [AST.While node]
	:	While condition topExpressionNull		{ node = AST.Node.While($condition.node, $topExpressionNull.node); }
	;

simpleExpression returns [AST.Node node]
	:	{ this.isfunction }? StackReference expressionGroup
		{
			node = AST.Node.UserDefInvoke(this.function, $expressionGroup.node);
		}
	|	expr=termExpression	{ node = $expr.node; }
		(   
			expressionGroup
			{
				if( ($termExpression.node is AST.Identifier) )
				{
					node = AST.Node.UserDefInvoke($termExpression.node, $expressionGroup.node);
					
				}
				else
				{
					throw new ParseException("non-function", false);
				}
				
			}
		)?
	;

termExpression returns [AST.Node node]
	:	term { node = $term.node; }
		(LSBracket i=expressionList RSBracket
			{ node = AST.Node.Indexing(node, $i.node);  } 
		)*
	;

term returns [AST.Node node]
	:	LP expressionList RP				
		{ 
			switch($expressionList.node.Length)
			{
				case 0:
					node = AST.Node.NullConstant();
					break;
				case 1:
					node = $expressionList.node.Items.First.Value;
					if(node is AST.Identifier)
					{
						((AST.Identifier)node).IsEnclosed = true;
					}
					break;
				
				default:
					node = AST.Node.Strand($expressionList.node.Items);
					break;
			}
		}
	|	expressionGroup						{ node = $expressionGroup.node; }
	|	constant							{ node = $constant.node; }
	|	variableName						{ node = $variableName.node; }
	;

constant returns [AST.Node node]
	:	numericConstantList					{ node = $numericConstantList.node; }					
	|	symbolConsantList					{ node = $symbolConsantList.node; }
	|	characterConstant					{ node = $characterConstant.node; }
	;


numericConstant returns [AST.Constant number]
	:	Int									{ number = AST.Node.IntConstant($Int.Text); }
	|	Float								{ number = AST.Node.FloatConstant($Float.Text); }
	|	Inf									{ number = AST.Node.InfConstant($Inf.Text); }
	;
	
numericConstantList returns [AST.Node node]
	@init { AST.ConstantList list = AST.Node.ConstantList(); }
	:	(numericConstant { list.AddLast($numericConstant.number); } )+
		{ node = list; }
	;
	
symbolConsantList returns [AST.Node node]
	@init { AST.ConstantList list = AST.Node.ConstantList(); }
	:	(SymbolConstant { list.AddLast(AST.Node.SymbolConstant($SymbolConstant.Text)); } )+
		{ node = list; }
	;

characterConstant returns [AST.Node node]
	:	CharachterConstantSQ				{ node = AST.Node.SingeQuotedConstant($CharachterConstantSQ.text); }
	|	CharachterConstantDQ				{ node = AST.Node.DoubleQuotedConstant($CharachterConstantDQ.text); }
	;

variableName returns [AST.Identifier node]
 	:	SystemName							{ node = AST.Node.SystemName($SystemName.Text); }
		// User Name:
	|	QualifiedName						{ 
												node = AST.Node.QualifiedName($QualifiedName.Text);

												if(this.isdependency)
												{
													this.variableAccessing.Add(node);
												}
											}
	|	UnqualifiedName						{ 
												node = AST.Node.UnQualifiedName($UnqualifiedName.Text);

												if(this.isdependency)
												{
													this.variableAccessing.Add(node);
												}
											}
	;