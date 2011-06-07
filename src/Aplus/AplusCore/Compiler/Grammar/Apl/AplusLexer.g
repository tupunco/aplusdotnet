lexer grammar AplusLexer;

options {
	language = CSharp2;
}

tokens {
	SystemCommand;
	SystemCommandArgument;
}

@lexer::namespace { AplusCore.Compiler.Grammar.Apl }

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
RAnd:			'^/';
RMax:			'\u00D3/';
RMin:			'\u00C4/';
RMultiply:		'\u00AB/';
ROr:			'\u00A9/';
SAdd:			'+\\';
SAnd:			'^\\';
SMax:			'\u00D3\\';
SMin:			'\u00C4\\';
SMultiply:		'\u00AB\\';
SOr:			'\u00A9\\';

AbsoluteValue:		'\u00CD';
Ceiling:			'\u00D3';
Count:				'\u0023';
DefaultFormat:		'\u00EE';
Depth:				'\u00BD';
Disclose:			'\u003E';
Enclose:			'\u003C';
Execute:			'\u00E2';
Exponential:		'\u002A';
Floor:				'\u00C4';
GradeDown:			'\u00E7';
GradeUp:			'\u00E8';
Identity:			'\u002B';
Interval:			'\u00C9';
ItemRavel:			'\u0021';
MapIn:				'\u00E0';
NaturalLog:			'\u00F0';
Negate:				'\u002D';
Not:				'\u007E';
Null:				'\u00DD';
Pack:				'\u00C2';
PartitionCount:		'\u00DA';
PiTimes:			'\u00CF';
Print:				'\u00D5';
Rake:				'\u00C5';
Ravel:				'\u002C';
Raze:				'\u00D8';
Reciprocal:			'\u00DF';
Reverse:			'\u00F7';
Roll:				'?';
Right:				'\u00DB';
SeparateSymbols:	'\u002E';
Shape:				'\u00D2';
Sign:				'\u00AB';
Signal:				'\u00D9';
Stop:				'^';
Transpose:			'\u00F4';
Unpack:				'\u00CE';
Value:				'%';

//Dyadic Symbol:

//And:			'^';
Equal:			'=';
Expand:			'\\';
GTE:			'\u00A6';
LTE:			'\u00A4';
NotEqual:		'\u00A8';
Or:				'\u00A9';
Replicate:		'/';
Result:			'\u00FB';

IPAddMultiply:	'+.\u00AB';
IPMaxAdd:		'\u00D3.+';
IPMinAdd:		'\u00C4.+';
OPAdd:			'\u00CA.+';
OPDivide:		'\u00CA.\u00DF';
OPEqual:		'\u00CA.=';
OPGT:			'\u00CA.>';
OPGTE:			'\u00CA.\u00A6';
OPLT:			'\u00CA.<';
OPLTE:			'\u00CA.\u00A4';
OPMax:			'\u00CA.\u00D3';
OPMin:			'\u00CA.\u00C4';
OPMultiply:		'\u00CA.\u00AB';
OPNotEqual:		'\u00CA.\u00A8';
OPResidue:		'\u00CA.|';
OPSubstract:	'\u00CA.-';
OPPower:		'\u00CA.\u002A';

// Bitwise operators:
BWNot:			'\u007E\u00AE';
BWAnd:			'^\u00AE';
BWOr:			'\u00A9\u00AE'; // Bitwise cast
BWLT:			'<\u00AE';
BWLTE:			'\u00A4\u00AE';
BWGT:			'>\u00AE';
BWGTE:			'\u00A6\u00AE';
BWEqual:		'=\u00AE';
BWNotEqual:		'\u00A8\u00AE';

//Miscelonus:
StackReference:	'&';

LP:				'(' {mode++;};
RP:				')' {mode--;};
LSBracket:		'[';
RSBracket:		']';
SemiColon:		';';
Colon:			':';
Each:			'\u00A1';
Rank:			'@';

Blank:			WhiteSpace {$channel = HIDDEN;};
LSB:			'{' {mode++;};
RSB:			'}' {mode--;};
Comment:		'\u00E3' (~'\n')* { $channel = HIDDEN; };
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
	:	'\u00A2'? Number
	;
	
fragment Exponent
	:	('e'|'E') ('-' | '+')? Number
	;

Float
	:	'\u00A2'? Number Exponent
	|	'\u00A2'? '.' Number Exponent?
	|	'\u00A2'? Number '.' Number? Exponent?
	;

Inf
	:	'\u00A2'? 'Inf'
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
