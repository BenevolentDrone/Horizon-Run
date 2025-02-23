lexer grammar StanleyLexer;

// Compound keywords

//Had (been) 4
HAD_BEEN_GREATER: 'had been greater than';
HAD_BEEN_LESS: 'had been less than';
HAD_BEEN_AT_LEAST: 'had been at least';
HAD_BEEN_AT_MOST: 'had been at most';

//Became 3
BECAME_GREATER: 'became greater than';
BECAME_LESS: 'became less than';
BECAME_AT_LEAST: 'became at least';
BECAME_AT_MOST: 'became at most';

//This 3
THIS_WAS_REPEATED: 'This was repeated';  // Put this first since it contains 'was'
THIS_IS_HOW: 'This is how';

//Was 3
WAS_GREATER: 'was greater than';
WAS_LESS: 'was less than';
WAS_AT_LEAST: 'was at least';
WAS_AT_MOST: 'was at most';

//It 3
IT_WAS_EXPECTED: 'It was expected';

//As 3
AS_LONG_AS: 'As long as';

//Was 2
WAS_INSERTED: 'was inserted';
WAS_COMBINED: 'was combined';
WAS_COUNTED: 'was counted';
WAS_CLEARED: 'was cleared';
WAS_REMOVED: 'was removed';
WAS_LOADED: 'was loaded';  // For program loading
WAS_ADDED: 'was added';
WAS_NOT: 'was not';

//Has / Had 2
HAS_HAPPENED: 'has happened';
HAD_INSERTED: 'had inserted';
HAD_COMBINED: 'had combined';
HAD_COUNTED: 'had counted';
HAD_CLEARED: 'had cleared';
HAD_REMOVED: 'had removed';
HAD_ADDED: 'had added';
HAD_BEEN: 'had been';

//Various 2
FOR_EACH: 'For each';
THERE_WAS: 'There was';
OTHERWISE_IF: 'Otherwise if';
ELSE_IF: 'Else if';
BECAME_EMPTY: 'became empty';
AT_POSITION: 'at position';

// Types (longer words)
TYPE_PROGRAM: 'program';  // New type for programs
TYPE_REAL_NUMBER: 'real number';  // For floating point values
TYPE_NUMBER: 'number';  // For integers
TYPE_OBJECT: 'object';
TYPE_ACTION: 'action';
TYPE_EVENT: 'event';
TYPE_LIST: 'list';
TYPE_TEXT: 'text';
TYPE_FACT: 'fact';

// Nouns - Time units
MILLISECONDS: 'milliseconds';
SECONDS: 'seconds';
MINUTES: 'minutes';
TIMES: 'times';
TIME: 'Time';

// Nouns - Positions
FIRST: [Ff]'irst';
LAST: [Ll]'ast';

// Nouns - Data
PROGRAM: 'Program';  // Add before VALUE since it's more specific
VALUE: [Vv]'alue';
COUNT: 'count';

// Nouns - References
THIS: 'this';

// Verbs (longer words)
OTHERWISE: 'Otherwise';
EXECUTED: 'executed';
INSERTED: 'inserted';
HAPPENED: 'happened';
HANDLED: 'handled';
COUNTED: 'counted';
CLEARED: 'cleared';
REMOVED: 'removed';
PASSED: 'passed';
CALLED: 'called';
BECAME: 'became';
ADDED: 'added';
WHILE: 'While';
ELSE: 'Else';
BEEN: 'been';
ONCE: 'Once';
DONE: 'done';
END: 'End';  

// Conjunctions (short words)
UNTIL: 'until';
THAT: 'that';
AND: 'and';
FOR: 'for';
IF: 'If';

// Prepositions (short words)
WITH: 'with';
INTO: 'into';
FROM: 'from';
TO: 'to';
OF: 'of';
IN: 'in';
ON: 'on';
AT: 'at';
BY: 'by';

// Articles (shortest words)
THE: [Tt]'he';
AN: 'an';
A: 'a';

// Short state verbs
WAS: 'was';
HAD: 'had';
HAS: 'has';

// Punctuation 
POSSESSIVE: '\'s';  // Put this before APOSTROPHE to match 's first
APOSTROPHE: '\'';
COLON: ':';
COMMA: ',';
LPAREN: '(';
RPAREN: ')';
LBRACKET: '[';
RBRACKET: ']';

// Operators (by length)
GREATER_THAN_OR_EQUAL: '>=';
LESS_THAN_OR_EQUAL: '<=';
NOT_EQUAL: '!=';
GREATER_THAN: '>';
LESS_THAN: '<';
EQUAL: '==';
PLUS: '+';
MINUS: '-';
MULTIPLY: '*';
DIVIDE: '/';
POWER: '^';
MOD: '%';

// Literals
REAL_NUMBER: '-'? [0-9]+ '.' [0-9]+;  // Must have decimal point
NUMBER: '-'? [0-9]+;  // Integer only
STRING: ('"' ~["]* '"') | ('\'' ~['\n\r]* '\'');  // Match anything except quotes and newlines
BOOLEAN: 'true' | 'false';

// Verbs and identifiers
VERB: [a-zA-Z]+[e][d];  // Match words ending in -ed
IDENTIFIER: [a-zA-Z][a-zA-Z0-9_]*;

// DOT after identifiers to prevent it from being part of them
DOT: '.';

// Comments and whitespace
COMMENT: '//' ~[\r\n]* -> skip;
WS: [ \t\r\n]+ -> skip;
