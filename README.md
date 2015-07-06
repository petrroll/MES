__MES__ is a mathematical expressions evaluator (let's just say a clever calculator) written in .NET. While being fairly simple it's heavily focused on being generic, modular, and extensible. It's implemented in a Console application for showcase but can easily be stripped down and used as a standalone library. 

##Features:
* Binary operators with arbitrary priorities.
* Pre-defined functions (sin, cos, if, ...).
* Modular Parser-Tokenizer-TokenFactory-ExpressionTreeBuilder system.
* Comprehensive Exception system.
* User defined functions with abritrary arguments.
* User definied variables support.
* Robust Parser.

##Syntax:
MES should be able to handle any valid math expression in infix notation. Whitespace between operators, values, and functions is recomended for better readability but is not required by Parser. Unknown characters (@, &, ...) are skipped by default but can be set to throw an error.

In addition to plain math expressions without external dependencies MES supports custom functions and variables as well. Descriptors of both variables and functions (as well as functions' arguments) must consist of only Unicode letters and underscore character. 

Argumenents are separated by semilocolon followed by optional whitespace. Last empty argument is ignored. Following funcion is, therefore, called with only two arguments: 2 and 5.
```
fun(2;5;)
```


A custom variable Pi with value of 3.14 is assigned as:
```
Pi = 3.14
```
A custom funcion FallSpeed with two parameters g and time is assigned as:
```
FallSpeed(g; time) = g*(t*t)
```
And used as:
```
FallSpeed(9.81; 5)
```

##Modularity:
MES is composed of a number of independend modules all of which can be easily replaced. 

##Issues:
* Evaluation of deeply nested expressions isn't optimized.
* Serialization of Expression tree would be difficult due to generic nature of Tokens.
* Test coverage is very limited.

----
###Lisense:
Do whatever you wish with it.
