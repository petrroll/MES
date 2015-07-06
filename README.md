###About:
__MES__ is a mathematical expressions evaluator (let's just say a clever calculator) written in .NET. While being fairly simple it's heavily focused on being generic, modular, and extensible. It's implemented in a Console application for showcase but can easily be stripped down and used as a standalone library. 

###Features:
* Binary operators (+, -, *, ...).
* Goniometric functions (sin, cos, tan).
* Automatic bool-double retyping (true : 1; false : 0).
* Custom variables support (useful for e.g. constans).
* Functions with arbitrary number of arguments support.
  * Proof-of-conncept 'if(condition > 0; true-result; false-result)' function.
  * Support for custom functions with arbitrary argument names.
* Modular 'Parser-Tokenizer-Expression tree builder' system..
  * Adding matrixes support requires only: 
    * Adding some logic to TokenFactory and Parser
	* Implementing matrix operators tokens.
* Comprehensive Exception system 
  * With reasonable Exceptions messages indicating where the error is.
* Robust Parser engine.

###Issues:
- Tokens hierarchy and TokenFactory are not best designed.

###Lisense:
Do whatever you wish with it.
