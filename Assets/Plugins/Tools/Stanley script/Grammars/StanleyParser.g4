parser grammar StanleyParser;

options
{
	tokenVocab = StanleyLexer;
}

// Top level description

script
	: storyHeader statement+
	;

storyHeader
	: STORY object //subject
	;

statement
	: defineStatement 
	| commandStatement
	;
	//| eventStatement
	//| assertStatement;

// Define statements

defineStatement
	: REFER_TO defineSubject AS object
	;

defineSubject
	: pluralSubjectsExpression
	| subjectExpression
	;

// Command statements

commandStatement
	: actionStatement
	| timeStatement;

timeStatement
	: timeExpression (AND timeExpression)* PASSED
	;

// Event statements

// Assert statements

//assertStatement
//	: subjectExpression SHOULD_BE objectExpression
//	;

// Action statements

actionStatement
	: pluralSubjectsExpression WERE actionExpression
	| subjectExpression (WERE | WAS) actionExpression //Remember that a variable may contain an array so "WERE" is still appliable
	| pluralSubjectsExpression HAVE actionExpression
	| subjectExpression (HAVE | HAS) actionExpression //Same for HAVE
	;

actionExpression
	: actionWithArguments
	| action
	;

actionWithArguments
	: action objectArgument
	;

// Action arguments

objectArgument
	: (TO | AT)? pluralObjectsExpression
	| (TO | AT)? objectExpression
	;

// Subject expressions

pluralSubjectsExpression
	: subject (AND subject)+
	;

subjectExpression
	: selectedSubject
	| subject
	;

// Selected subject expressions

selectedSubject
	: subjectSelectedByQuality
	| subjectSelectedInRelation
	;

subjectSelectedByQuality
	: A selectionAdjective assertAdjective* OF subject
	;

subjectSelectedInRelation
	: THE relativeSelectionAdjective TO subjectExpression subject
	;

// Object expressions

pluralObjectsExpression
	: integer object //objectExpression //Commenting this out. It would be weird to have "5.0 THE closiest TO Player chairs" as an object //Maybe?
	| float object
	;

objectExpression
	: selectedObject
	| object
	;

// Selected object expressions

selectedObject
	: objectSelectedByQuality
	| objectSelectedInRelation;

objectSelectedByQuality:
	A selectionAdjective assertAdjective* OF object;

objectSelectedInRelation:
	THE relativeSelectionAdjective TO subjectExpression object; //Keep in mind: TO subject, not TO object

// Selection adjectives

selectionAdjective
	: ID //RANDOM | WEAKEST | STRONGEST
	;

relativeSelectionAdjective
	: ID //CLOSIEST | FURTHEST
	;

assertAdjective
	: ID //UNIQUE
	;

// Time expressions

timeExpression
	: (integer | float) timeStep
	;

timeStep
	: SECONDS
	| MINUTES
	;

// Simple subjects, objects and actions

//(SUFFIX_N | SUFFIX_D | SUFFIX_ED)? //Suffixes in LL parsers just don't work. Gonna have to use aliases (like 'give', 'given', 'gave') instead.

subject
	: importVariableLiteral
	| runtimeVariableLiteral
	;

object
	: ID 				//runtimeVariableLiteral //Just to differentiate object values from variables
	| STRING_LITERAL
	;

action
	: ID
	;

// Literals

importVariableLiteral
	: DOLLAR_SYMB ID
	;

runtimeVariableLiteral
	: ID
	| STRING_LITERAL
	;

integer
	: DECIMAL_LITERAL
	;

float
	: REAL_LITERAL
	;