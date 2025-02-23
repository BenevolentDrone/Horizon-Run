parser grammar StanleyParser;

options { tokenVocab=StanleyLexer; }

// Root rule
program
    : statement*
    ;

// Statements (highest precedence)
statement
    : declarationStatement
    | actionStatement
    | assignmentStatement
    | eventHandlerStatement
    | delegateHandlerStatement
    | assertStatement
    | timeStatement
    | eventStatement
    | loopStatement
    | listOperationStatement
    | programStatement
    | conditionalStatement
    | COMMENT
    ;

// Declaration statements
// Examples:
// There was a number called Counter.                  // Number variable
// There was a text called Message.                    // Text variable
// There was a boolean called IsReady.                 // Boolean variable
// There was an object Button.                         // Object declaration
// There was an object called Button.                  // Same as above
// There was a list Items.                            // List declaration
// There was a list called Items.                     // Same as above
// There was an event OnComplete.                     // Event declaration
// There was an event called OnComplete.              // Same as above
// There was an event Display's OnUpdate.             // Event on object
// There was an event called Display's OnUpdate.      // Same as above
declarationStatement
    : THERE_WAS article? typeName (CALLED)? subjectReference DOT
    ;


// Action statements
// Examples:
// Player's Jump was executed.                        // Simple action execution
// Enemy's Move has started with Speed.              // Action with single argument
// Player's Attack has started with Damage and Type.  // Action with multiple arguments
// Ball's Move has started with X and Y and Speed.   // Action with three arguments
// Game's Load has started with "save1.dat".         // Action with string argument
// Timer's Start has started with 60.                // Action with number argument
// Player's Jump was executed with Speed.            // Simple action execution with argument
// Enemy's Move has started with Speed and Direction. // Action with multiple arguments
actionStatement
    : subjectReference WAS EXECUTED (actionArguments)? DOT
    | subjectReference HAS VERB (actionArguments)? DOT
    ;

// Assignment statements
// Examples:F
// The Counter was 5.
// Counter had been 5.
// The Player's Health became 100.
// First value from Player's Items was 42.
// The Last value from The Player's Items became Result.
// The Value at position 2 from Player's Items had been Previous.
assignmentStatement
    : subject (WAS | (HAD BEEN) | BECAME) expression DOT
    ;

// Event handler statements
// Example: Once OnComplete happened:
//   Counter became Counter + 1.
// End.
//
// Example: Once Button's OnClick happened:
//   Message became "Click detected".
// End.
eventHandlerStatement
    : ONCE subjectReference HAPPENED COLON statement* END DOT
    ;

// Delegate handler statements
// Example: This is how Handler was done:
//   Message became "Handler finished".
// End.
//
// Example: This is how Button's action was handled:
//   Message became "Action completed".
// End.
delegateHandlerStatement
    : THIS_IS_HOW subjectReference WAS (DONE | HANDLED) COLON statement* END DOT
    ;

// Assert statements
// Example: It was expected that Counter was greater than zero.
assertStatement
    : IT_WAS_EXPECTED THAT condition DOT
    ;

// Time statements
// Examples:
// Time passed for 100 milliseconds.
// Time passed until Display's OnUpdate happened.
// Display's OnUpdate has happened.
// It was expected that Message was 'Display updated'.
timeStatement
    : TIME PASSED FOR timeExpression DOT
    | TIME PASSED UNTIL subjectReference HAPPENED DOT
    ;

// Event statements
// Examples:
// The Button's Click has happened.                      // Button click event
// The Player's Death has happened.                      // Player death event
// The Game's Start has happened.                        // Game start event
// The Enemy's Spawn has happened.                       // Enemy spawn event
eventStatement
    : subjectReference HAS_HAPPENED DOT                 // Event occurrence
    ;

// Loop statements
// Example: This was repeated 5 times:
//   Counter became Counter + 1.
// End.
//
// Example: As long as Counter was less than 10:
//   Counter became Counter + 1.
// End.
//
// Example: For each Item in Items:
//   Message became Item.
// End.
loopStatement
    : THIS_WAS_REPEATED expression TIMES COLON statement* END DOT           // Count-based loop
    | AS_LONG_AS condition COLON statement* END DOT                         // While loop
    | FOR_EACH IDENTIFIER IN IDENTIFIER COLON statement* END DOT            // For-each loop
    ;

// List operation statements
// Examples:
// Value 5 was added to Player's Items.
// Value 5 was inserted at position 2 in Player's Items.
// Value at position 2 was removed from Player's Items.
// Player's Items was combined with OtherList.
// The Player's Items became empty.
listOperationStatement
    : VALUE expression (WAS_ADDED | HAD_ADDED) TO subjectReference DOT                    // Push value to list
    | VALUE expression (WAS_INSERTED | HAD_INSERTED) AT_POSITION expression IN subjectReference DOT  // Insert at index
    | VALUE AT_POSITION expression (WAS_REMOVED | HAD_REMOVED) FROM subjectReference DOT  // Remove at index
    | subjectReference (WAS_COMBINED | HAD_COMBINED) WITH subjectReference DOT           // Concatenate lists
    | subjectReference BECAME_EMPTY DOT                                                  // Clear list
    ;

// Program statement
// Examples:
// Program "MyScript.txt" was loaded into ScriptA.
// Program from "Scripts/test.txt" was loaded into TestScript.
programStatement
    : PROGRAM (FROM)? STRING WAS_LOADED subjectReference DOT               // Load program
    ;

// Conditional statements
// Examples:
// If Counter was greater than 5:
//   Message became "High score!".
// End.
//
// If Counter was greater than 5:
//   Message became "High score!".
// Otherwise:
//   Message became "Try again.".
// End.
conditionalStatement
    : IF condition COLON
      statement*
      elseIfClause*
      elseClause?
      END DOT
    ;

// Subject definitions (high precedence)
// Examples:
// The Counter                                         // Simple reference
// Player's Items                                      // Field reference
// First value from Player's Items                     // List value access
// Last value from The Player's Items                  // List value with article
// Value at position 2 from Player's Items            // List value at index
subject
    : subjectReference                               // Simple or field reference with optional article
    | listValue                                      // List value access
    ;

// Clauses (medium-high precedence)
// Examples:
// Otherwise if Counter was greater than 5:
//   Message became "High score!".
elseIfClause
    : (ELSE_IF | OTHERWISE_IF) condition COLON
      statement*
    ;

// Examples:
// Otherwise:
//   Message became "Try again.".
elseClause
    : (ELSE | OTHERWISE) COLON
      statement*
    ;

// Action arguments
// Examples:
// with Speed                                         // Single argument
// with Speed and Direction                           // Two arguments
// with X and Y and Speed                            // Three arguments
// with "save1.dat"                                  // String argument
// with 60                                           // Number argument
// into Player's Items                               // Single argument
// into Player's Items and Enemy's Items             // Two arguments
actionArguments
    : (preposition argumentList)+
    ;

// Examples:
// Speed                                              // Single expression
// Speed and Direction                                // Multiple expressions
argumentList
    : expression (AND expression)*
    ;

// Expressions and conditions (medium-low precedence)
// Examples:
// 42                                                  // Number literal
// 3.14                                               // Real number literal
// "Hello"                                            // String literal
// Counter                                            // Variable reference
// Player's Health                                    // Field reference
// (Counter * 2) + 1                                  // Parenthesized expression
expression
    : REAL_NUMBER                                  // Real number literal
    | NUMBER                                       // Numeric literal
    | STRING                                       // String literal
    | COUNT FROM subjectReference                 // List count
    | subjectReference                            // Variable or field reference
    | listValue                                   // List value access
    | expression operation expression             // Binary operation
    | LPAREN expression RPAREN                    // Parenthesized expression
    ;

// Conditions (medium-low precedence)
// Examples:
// Counter was greater than 5                         // Simple comparison
// Health was less than 100                          // Less than
// Score had been at least 1000                      // Had been comparison
// Lives became at most 3                            // Became comparison
condition
    : expression comparison? expression
    ;

// Time expressions (low precedence)
// Examples:
// 100 milliseconds
// 5 seconds and 500 milliseconds
// 2 minutes and 30 seconds
timeExpression
    : NUMBER timeUnit (AND timeExpression)?
    ;

// References and values (low precedence)
// Examples:
// The Counter                                         // Simple reference
// Player's Items                                      // Field reference
// The Player's Character's Items                      // Nested field reference
subjectReference
    : article? IDENTIFIER (POSSESSIVE IDENTIFIER)*   // The Counter, Player's Items, The Player's Character's Items
    ;

// Examples:
// First value from Player's Items                     // First element
// Last value from The Player's Items                  // Last element
// Value at position 2 from Player's Items            // Element at index
listValue
    : article? FIRST VALUE FROM subjectReference             // The First value from Player's Items
    | article? LAST VALUE FROM subjectReference             // The Last value from Player's Items
    | article? VALUE AT_POSITION expression FROM subjectReference  // The Value at position 2 from Player's Items
    ;

// Basic building blocks (lowest precedence)
article
    : THE
    | AN
    | A
    ;

preposition
    : WITH
    | INTO
    | FROM
    | TO
    | OF
    | IN
    | ON
    | AT
    | BY
    ;

timeUnit
    : MILLISECONDS
    | SECONDS
    | MINUTES
    ;

operation
    : GREATER_THAN_OR_EQUAL
    | LESS_THAN_OR_EQUAL
    | NOT_EQUAL
    | GREATER_THAN
    | LESS_THAN
    | EQUAL
    | PLUS
    | MINUS
    | MULTIPLY
    | DIVIDE
    | POWER
    | MOD
    ;

comparison
    : HAD_BEEN_GREATER
    | HAD_BEEN_LESS
    | HAD_BEEN_AT_LEAST
    | HAD_BEEN_AT_MOST
    | BECAME_GREATER
    | BECAME_LESS
    | BECAME_AT_LEAST
    | BECAME_AT_MOST
    | WAS_GREATER
    | WAS_LESS
    | WAS_AT_LEAST
    | WAS_AT_MOST
    | WAS_NOT
    | HAD_BEEN
    | BECAME
    | WAS
    ;

// Types
typeName
    : TYPE_PROGRAM                                                      // Add first for highest precedence
    | TYPE_REAL_NUMBER                                                 // Real numbers (float)
    | TYPE_NUMBER                                                      // Integers
    | TYPE_OBJECT
    | TYPE_ACTION
    | TYPE_EVENT
    | TYPE_LIST
    | TYPE_TEXT
    | TYPE_FACT
    ;
