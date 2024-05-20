lexer grammar StanleyLexer;

channels
{
	STANLEY_COMMENT,
	ERRORCHANNEL
}

// SKIP

// DO NOT MODIFY THE CODE BELOW UNLESS YOU HAVE MALICIOUS INTENTIONS

SPACE: [ \t\r\n]+ -> skip;
COMMENT: '/*!' .+? '*/' -> skip;
COMMENT_INPUT: '/*' .*? '*/' -> skip;
LINE_COMMENT:
	(
		('//' [ \t]* | '#') ~[\r\n]* ('\r'? '\n' | EOF)
		| '//' ('\r'? '\n' | EOF)
	) -> skip;

// Keywords: common keywords

REFER_TO: 'REFER TO';
STORY: 'STORY ABOUT';
SHOULD_BE: 'SHOULD BE';

// Keywords: articles

A: 'A';
THE: 'THE';

// Keywords: conjunctions

AND: 'AND';
AT: 'AT';

// Keywords: prepositions

AS: 'AS';
OF: 'OF';
TO: 'TO';

// Keywords: verbs

HAD: 'HAD';
HAS: 'HAS';
HAVE: 'HAVE';
PASSED: 'PASSED';
WAS: 'WAS';
WERE: 'WERE';

// Keywords: adjectives

CLOSIEST: 'CLOSIEST';
FURTHEST: 'FURTHEST';
RANDOM: 'RANDOM';
STRONGEST: 'STRONGEST';
UNIQUE: 'UNIQUE';
WEAKEST: 'WEAKEST';

// Keywords: time steps

MINUTES: 'MINUTES';
SECONDS: 'SECONDS';

// Keywords: suffixes
//SUFFIX_D: 'D';
//SUFFIX_ED: 'ED';
//SUFFIX_N: 'N';
//SUFFIX_S: 'S';

// Operators. Arithmetics

STAR: '*';
DIVIDE: '/';
MODULE: '%';
PLUS: '+';
MINUS: '-';

// Operators. Comparation

EQUAL_SYMBOL: '=';
GREATER_SYMBOL: '>';
LESS_SYMBOL: '<';
EXCLAMATION_SYMBOL: '!';

// Constructors symbols

DOT: '.';
LR_BRACKET: '(';
RR_BRACKET: ')';
COMMA: ',';
SEMI: ';';
AT_SIGN: '@';
ZERO_DECIMAL: '0';
ONE_DECIMAL: '1';
TWO_DECIMAL: '2';
SINGLE_QUOTE_SYMB: '\'';
DOUBLE_QUOTE_SYMB: '"';
REVERSE_QUOTE_SYMB: '`';
COLON_SYMB: ':';
DOLLAR_SYMB: '$';

fragment QUOTE_SYMB:
	SINGLE_QUOTE_SYMB
	| DOUBLE_QUOTE_SYMB
	| REVERSE_QUOTE_SYMB;

// Literal Primitives

STRING_LITERAL: DQUOTA_STRING | SQUOTA_STRING | BQUOTA_STRING;
DECIMAL_LITERAL: DEC_DIGIT+;
REAL_LITERAL
	: DEC_DIGIT+ '.' DEC_DIGIT*
	| '.' DEC_DIGIT+
	;

// Identifiers

ID: ID_LITERAL;
// DOUBLE_QUOTE_ID:                  '"' ~'"'+ '"';
REVERSE_QUOTE_ID: BQUOTA_STRING;

// Fragments for Literal primitives

fragment ID_LITERAL:
	[a-zA-Z_][a-zA-Z_0-9]*;
fragment DQUOTA_STRING:
	'"' ('\\' . | '""' | ~('"' | '\\'))* '"';
fragment SQUOTA_STRING:
	'\'' ('\\' . | '\'\'' | ~('\'' | '\\'))* '\'';
fragment BQUOTA_STRING: '`' ( ~'`' | '``')* '`';
fragment DEC_DIGIT: [0-9];

// Last tokens must generate Errors

ERROR_RECONGNIGION: . -> channel(ERRORCHANNEL);