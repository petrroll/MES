__MES__ is a mathematical expressions evaluator (let's just say a fancy calculator) written in .NET. While being fairly simple it's heavily focused on being generic, modular, and extensible. It's wrapped in a console application for a showcase but can easily be stripped down and used as a standalone library. 

##Features:
* Binary operators with arbitrary priorities.
* Pre-defined functions (sin, cos, if, ...).
* Modular Parser-Tokenizer-TokenFactory-ExpressionTreeBuilder system.
* Comprehensive Exception system.
* User defined functions with abritrary arguments.
* User definied variables support.
* Robust Parser.

##Syntax:
MES should be able to handle any valid math expression in infix notation. Whitespace between operators, values, and functions is recomended for better readability but is not required. Unknown characters (@, &, ...) are skipped by default but can be set to throw an error.

In addition to plain math expressions without external dependencies MES supports custom functions and variables as well. Descriptors of both variables and functions (as well as functions' arguments) must consist of only Unicode letters and underscore character. 

Individual argumenents are separated by a semilocolon followed by an optional whitespace. Last empty argument is ignored. Nested arguments (i.e functions and expressions inside arguments) are supported, recursion is not.

###Examples:
Call a function 'fun' with two parameters '2' and '5'.
```
fun(2;5;)
```

Assign a custom variable 'Pi' with value of '3.14':
```
Pi = 3.14
```

Assign a custom function 'areaOfCircle' with one parameter 'diameter':
```
areaOfCircle(diameter) = (diameter / 2)*(diameter / 2)*(Pi)
```

Use just assigned function to compute area of '7' circles with diameter of '5' meters:
```
areaOfCircle(5) * 7
```

Assign a custom funcion 'FallSpeed' with two parameters 'g' and 'time':
```
FallSpeed(g; time;) = g*(time*time)
```

Use that function to compute fall speed for '5' seconds fall on Earth (notice that last empty argument is ignored):
```
FallSpeed(9.81; 5;)
```

##Modularity:
MES is composed of a number of independend modules all of which can be easily replaced. The most low level one is Parser that takes a string and cuts it into an array of substrings that fall into one of following group (number, descriptor, operator). Then, Tokenizer reads the array and through TokenFactory creates a linear array of Tokens out of it. And lastly linear array of Tokens is build into a true expression tree in ExpTreeBuilder. All of these objects are controlled and interconnected by a Controller that also handles special expressions such as assigning a custom function or variable.

Due to aforementioned modular nature extending MES is an easy task. For example adding support for matrixes would require only some little changes either to Parser or Tokenizer and adding Matrix (and matrix operations) Tokens. Neither token factory nor ExpTree builder let alone Controller would have to changed. 

##Issues:
* Evaluation of deeply nested expressions isn't optimized.
* Serialization of Expression tree would be difficult due to generic nature of Tokens.
* Test coverage is very limited.

----
###Lisense:
Do whatever you wish with it.
