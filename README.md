###About:
__MES__ is a mathematical expressions evaluator (let's just say a clever calculator) written in .NET. While being fairly simple it's heavily focused on being generic, modular, and extensible. It's implemented in a Console application for showcase but can easily be stripped down and used as a standalone library. 

###Features:
* Binary operators (+, -, *, ...).
* Goniometric functions (sin, cos, tan).
* Automatic bool-double retyping (true : 1; false : 0).
* Variables memory (useful for constans).
* Support for (though right know just hard coded) custom functions with arbitrary number of arguments.
  * Proof-of-conncept 'if(condition > 0; true-result; false-result)' function.
* Completely modular 'Parser-Tokenizer-Expression tree builder' system where any component can be changed.
  * Enabling matrix calculations requires only adding some logic to Tokenizer and Parser and implementing matrix operators tokens.

###Issues:
- The controller class has never been refactored. 
- The generics are overused and the whole object architecture is a bit too bit complicated.
- The Storage variable and Storage function architecture is not very well thought through.
- Exceptions are not (yet) implemented so MES always tries to calculate something which results in nonsensical results on invalid input.

###Lisense:
Do whatever you wish with it.
