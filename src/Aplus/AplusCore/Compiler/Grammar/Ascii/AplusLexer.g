lexer grammar AplusLexer;

options {
	language = CSharp2;
}

tokens {
	SystemCommand;
	SystemCommandArgument;
}

@lexer::namespace { AplusCore.Compiler.Grammar.Ascii }

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
RMax:			'max/';
RMin:			'min/';
RMultiply:		'*/';
ROr:			'?/';
SAdd:			'+\\';
SAnd:			'&\\';
SMax:			'max\\';
SMin:			'min\\';
SMultiply:		'*\\';
SOr:			'?\\';

AbsoluteValue:		'|';
Ceiling:			'max';
Count:				'#';
DefaultFormat:		'form';
Depth:				'==';
Disclose:			'>';
Enclose:			'<';
Execute:			'eval';
Exponential:		'^';
Floor:				'min';
GradeDown:			'dng';
GradeUp:			'upg';
Identity:			'+';
Interval:			'iota';
ItemRavel:			'!';
MapIn:				'beam';
MatrixInverse:		'mdiv';
NaturalLog:			'log';
Negate:				'-';
Not:				'~';
Null:				'where';
Pack:				'pack';
PartitionCount:		'bag';
PiTimes:			'pi';
Print:				'drop';
Rake:				'in';
Ravel:				',';
Raze:				'pick';
Reciprocal:			'%';
Reverse:			'rot';
Roll:				'rand';
Right:				'rtack';
SeparateSymbols:	'dot';
Shape:				'rho';
Sign:				'*';
Signal:				'take';
Stop:				'&';
Transpose:			'flip';
Unpack:				'unpack';
Value:				'ref';

//Dyadic Symbol:

//And:			'&';
Equal:			'=';
Expand:			'\\';
GTE:			'>=';
LTE:			'<=';
NotEqual:		'~=';
Or:				'?';
Replicate:		'/';
Result:			':=';

IPAddMultiply:	'+.*';
IPMaxAdd:		'max.+';
IPMinAdd:		'min.+';
OPAdd:			'+.';
OPDivide:		'%.';
OPEqual:		'=.';
OPGT:			'>.';
OPGTE:			'>=.';
OPLT:			'<.';
OPLTE:			'<=.';
OPMax:			'max.';
OPMin:			'min.';
OPMultiply:		'*.';
OPNotEqual:		'~=.';
OPResidue:		'|.';
OPSubstract:	'-.';
OPPower:		'^.';

// Bitwise operators:
BWNot:			'bwnot';
BWAnd:			'bwand';
BWOr:			'bwor'; // Bitwise cast
BWLT:			'bwlt';
BWLTE:			'bwle';
BWGT:			'bwgt';
BWGTE:			'bwge';
BWEqual:		'bweq';
BWNotEqual:		'bwne';

//Miscelonus:
StackReference:	'\u00FE';

LP:				'(' {mode++;};
RP:				')' {mode--;};
LSBracket:		'[';
RSBracket:		']';
SemiColon:		';';
Colon:			':';
Each:			'each';
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
