lexer grammar AplusLexer;

options {
	language = CSharp2;
}

tokens {
	SystemCommand;
	SystemCommandArgument;
}

@lexer::namespace { AplusCore.Compiler.Grammar.Uni }

@header {
using System.Text;
using System.Collections.Generic;
using AplusCore.Compiler.Grammar;
}

@members {
	private int mode = 0;

	Queue<IToken> tokens = new Queue<IToken>();
	public override void Emit(IToken token) 
	{
		state.token = token;
		tokens.Enqueue(token);
	}
	public override IToken NextToken()
	{
		base.NextToken();
		if ( tokens.Count ==0 )
			return Token.EOF_TOKEN;
		return tokens.Dequeue();
	}

}
//Control keywords:

If:				'if';
Else:			'else';
Do:				'do';
While:			'while';
Case:			'case';

//Monadic Symbols:
RAdd:			'+/';
RAnd:			'&/';
RMax:			'M.+/';
RMin:			'M.-/';
RMultiply:		'*/';
ROr:			'|/';
SAdd:			'+\\';
SAnd:			'&\\';
SMax:			'M.+\\';
SMin:			'M.-\\';
SMultiply:		'*\\';
SOr:			'|\\';

AbsoluteValue:		'M.|';
Ceiling:			'M.+';
Count:				'#';
DefaultFormat:		'E.%';
Depth:				'==';
Disclose:			'>';
Enclose:			'<';
Execute:			'E.*';
Exponential:		'M.*';
Floor:				'M.-';
GradeDown:			'I.-';
GradeUp:			'I.+';
Identity:			'+';
Interval:			'I.#';
ItemRavel:			'S.!';
MapIn:				'F.!';
MatrixInverse:		'M.#';
NaturalLog:			'M.&';
Negate:				'-';
Not:				'!';
Null:				'A.<';
Pack:				'M.<';
PartitionCount:		'I.<';
PiTimes:			'M.^';
Print:				'S.-';
Rake:				'I.?';
Ravel:				',';
Raze:				'I.>';
Reciprocal:			'%';
Reverse:			'S.|';
Roll:				'M.?';
Right:				'A.>';
SeparateSymbols:	'Y.&';
Shape:				'S.?';
Sign:				'*';
Signal:				'S.+';
Stop:				'&';
Transpose:			'S.\\';
Unpack:				'M.>';
Value:				'^';

//Dyadic Symbol:

//And:			'&';
Equal:			'=';
Expand:			'\\';
GTE:			'>=';
LTE:			'<=';
NotEqual:		'!=';
Or:				'|';
Replicate:		'/';
Result:			':=';

IPAddMultiply:	'P.*';
IPMaxAdd:		'P.+';
IPMinAdd:		'P.-';
OPAdd:			'O.+';
OPDivide:		'O.%';
OPEqual:		'O.=';
OPGT:			'O.>';
OPGTE:			'O.>=';
OPLT:			'O.<';
OPLTE:			'O.<=';
OPMax:			'Q.+';
OPMin:			'Q.-';
OPMultiply:		'O.*';
OPNotEqual:		'O.!=';
OPResidue:		'Q.|';
OPSubstract:	'O.-';
OPPower:		'Q.*';

// Bitwise operators:
BWNot:			'B.!';
BWAnd:			'B.&';
BWOr:			'B.|'; // Bitwise cast
BWLT:			'B.<';
BWLTE:			'B.<=';
BWGT:			'B.>';
BWGTE:			'B.>=';
BWEqual:		'B.=';
BWNotEqual:		'B.!=';

//Miscelonus:
StackReference:	'?';

LP:				'(' {mode++;};
RP:				')' {mode--;};
LSBracket:		'[';
RSBracket:		']';
SemiColon:		';';
Colon:			':';
Each:			'~';
Rank:			'@';

Blank:			WhiteSpace {$channel = HIDDEN;};
LSB:			'{' {mode++;};
RSB:			'}' {mode--;};
Comment:		'//' (~'\n')* { $channel = HIDDEN; };
CR:				'\r' { $channel = HIDDEN; };

NewLine
	:			{mode > 0}?=> '\n' {$channel = HIDDEN;}
	|			{mode == 0}?=> '\n' {$channel = DEFAULT_TOKEN_CHANNEL;}
	;

fragment WhiteSpace
	:	' '
	|	'\t'
	;

fragment Digit
	:	'0' .. '9'
	;

fragment Number
	:	Digit+
	;

Int
	:	'-'? Number
	;
	
fragment Exponent
	:	('e'|'E') ('-' | '+')? Number
	;

Float
	:	'-'? Number Exponent
	|	'-'? '.' Number Exponent?
	|	'-'? Number '.' Number? Exponent?
	;

Inf
	:	'-'? 'Inf'
	;

fragment SQ:	'\'';

fragment DQ:	'"';

fragment Backslash:	'\\';

// NOTE: ANTLR hack rev 176
CharachterConstantSQ
	:	SQ ( SQ SQ | ('R' | ~SQ) )* SQ 
	;

// NOTE: ANTLR hack rev 176
CharachterConstantDQ
	:	DQ ( 'S' | ~DQ )* DQ
	;

fragment Alphabetic
	:	('a' .. 'z' | 'A' .. 'Z')
	;
 	
 fragment Alphanumeric
	:	(Alphabetic | Digit)
	;
 
 SymbolConstant
	:	'`'  (Alphanumeric | '_' | '.')*
	;
 	
UnqualifiedName
	:	Alphabetic (Alphanumeric | '_')*
	;

//Jav. szabaly 
QualifiedName
	:	(UnqualifiedName '.')+  UnqualifiedName
	|	'.' UnqualifiedName
	|   UnqualifiedName
	;

SystemName
	:	'_' UnqualifiedName
	;


SystemCommandName
	:	'$' ~('\n'|'\r')* {
		// split the line to 2 parts: command and argument
		// cut at the first space character
		string[] parts = $text.Split(new char[]{' '}, 2);

		// Emit the system command token
		Emit(new CommonToken(SystemCommand, parts[0]));

		// Emit the argument of the system command if there is any
		if(parts.Length > 1)
		{
			Emit(new CommonToken(SystemCommandArgument, parts[1].Trim()));
		}
	}
	;
