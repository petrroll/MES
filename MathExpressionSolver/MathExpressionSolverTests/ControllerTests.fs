namespace MathExpressionSolver.Tests

open Xunit
open FsUnit.Xunit

open System
open System.Collections.Generic
open MathExpressionSolver

open MathExpressionSolver.Tokens
open MathExpressionSolver.Controller
open MathExpressionSolver.Parser
open MathExpressionSolver.Builders

module ControllerIntegrationTests =

    let initController() = 
        System.Threading.Thread.CurrentThread.CurrentCulture <- new System.Globalization.CultureInfo("cs-CZ", false);
        
        let parser = new ExpressionParser()
        parser.SkipWhiteSpace <- true

        let customVariables = new Dictionary<string, double>();
        let customFunctions = new Dictionary<string, IFactorableCustFuncToken<double>>();

        let factory = new DoubleTokenFactory()
        factory.CustomVariables <- customVariables
        factory.CustomFunctions <- customFunctions 
        
        let tokenizer = new Tokenizer<double>() 
        tokenizer.TokenFactory <- factory 

        let treeBuilder = new ExpTreeBuilder<double>()

        let controller = new Controller<double>()
        controller.set_ExpTreeBuilder treeBuilder
        controller.set_Parser parser
        controller.set_Tokenizer tokenizer

        controller

    let bindController (controller : Controller<_>) testFunction expression =
        controller.ExecuteExpressionSafe expression |> testFunction

    let testInput (contr : Controller<_>) (expectedValue : string) (expression : string) = 
        let result = contr.ExecuteExpression expression
        result.ToString() |> should equal expectedValue

    let testValue (result : Result<_>) (value : double) = 
        result.Exception |> should be Null
        result.Value |> should (equalWithin 0.01) value

    let testAssigment (result : Result<_>) (value : double) (variableName : string) =
        result.Exception |> should be Null
        result.Value |> should (equalWithin 0.01) value
        result.AdditionalData |> should equal variableName

    let testNewFunction (result : Result<_>) (functionName : string) = 
        result.Exception |> should be Null   
        result.AdditionalData |> should equal functionName

    let testException (result : Result<_>) (exceptionMessage : string) =
        result.Exception.Message |> should equal exceptionMessage
        
    [<Fact>]
    let SimpleExpression() =
        let valueTester = bindController (initController()) testValue
        valueTester "2 + 3" 5.0
        valueTester "2 + 2.2" 4.2
        valueTester "1 + 2 / 3" (5.0/3.0)

    [<Fact>]
    let ParserTests() =
        let valueTester = bindController (initController()) testValue
        valueTester "3*(7+7)/2-2*6/7-(6+9)*8-(2+2/3*(6+exp(2*7-6*2)-8)+(2>1))" -107.306989780239
    
    [<Fact>]
    let BracketsTests() =
        let valueTester = bindController (initController()) testValue
        valueTester "((2+1)-(3+1)*2)" -5.0
        valueTester "(1 + 2) / 3" 1.0

    [<Fact>]
    let Variables() =
        let controller = initController ()
        let valueTester = bindController (controller) testValue
        let variableTester = bindController (controller) testAssigment

        variableTester "a=(3/6*5)" 2.5 "a"
        variableTester "b = 8" 8.0 "b"

        variableTester "Pi_approx = 3.4" 3.4 "Pi_approx"
        valueTester "3 - Pi_approx + 8" 7.6

        variableTester "c=(5 + 3)" 8.0 "c"
        valueTester "c" 8.0

        valueTester "exp(a) + c - 2*Pi_approx" 13.3824939607035


    [<Fact>]
    let IfsSimple() =
        let valueTester = bindController (initController()) testValue
        valueTester "if(1;2;3)" 2.0
        valueTester "if(0;2;3)" 3.0

    let prepareVariables (controller : Controller<double>) =
        controller.ExecuteExpression "a=(3/6*5)" |> ignore
        controller.ExecuteExpression "b=(5 + 3)" |> ignore

        controller.ExecuteExpression "Pi = 3.4"|> ignore
        controller

    [<Fact>]
    let AdvancedExpressions() =
        let controller = prepareVariables (initController())
        let valueTester = bindController controller testValue

        valueTester "if((exp(100)> Pi)*2;exp(a) + b - 2*Pi;2*3-b)" 13.3824939607035
        valueTester "(if((exp(100)> Pi)*2;exp(a) + b - 2*Pi;2*3-b))*2>1" 1.0

    [<Fact>]
    let FuncSimple() =
        let controller = initController()
        let valueTester = bindController controller testValue
        let funcTester = bindController controller testNewFunction

        funcTester "funA(a;b) = a + b" "funA"
        valueTester "funA(2;5)" 7.0

    [<Fact>]
    let Exceptions() =
        let controller = initController()
        let excTester = bindController controller testException

        excTester "2 + + 3" "Two binary operators next to each other: 2 + ,  + "
        excTester "2 + " "Binary operator: 2 +  doesn't have right side."
        excTester "+ 2" "Binary operator:  +  does't have left side"
        excTester "(2 + 2" "2 + 2; does not have an ending parenthese."
        excTester "sin()" "0. argument of sin() is empty."
        excTester "sin(2,3) " "Number of supplied (2) and required (1) arguments for sin() doesn't match."

    [<Fact>]
    let FuncsAdv() =
        
        let controller = initController()
        controller.ExecuteExpression "Pi = 3.4" |> ignore

        let valueTester = bindController controller testValue
        let funcTester = bindController controller testNewFunction
        
        funcTester "funC(a) = 5" "funC"
        valueTester "funC(3)" 5.0

        funcTester "funA(x; y) = x + x * y" "funA"
        valueTester "funA(3; 2)" 9.0
        valueTester "funA(3; 2)" 9.0

        funcTester "funB(height_in_m; diameter_in_cm) = Pi * ((diameter_in_cm / 100) * (diameter_in_cm / 100)) * height_in_m" "funB"

        valueTester "funB(Pi; sin(20))" 0.000963490199635007
        valueTester "funB(20; 8)" 0.4352


    [<Fact>]
    let FuncsAndVars() =
        
        let controller = initController()

        let valueTester = bindController controller testValue
        let funcTester = bindController controller testNewFunction
        let variableTester = bindController (controller) testAssigment

        funcTester "funA(x; y) = x + x * y" "funA"
        valueTester "funA(3; 2)" 9.0

        variableTester "x = 3" 3.0 "x"
        variableTester "y = funA(x; 5)" 18.0 "y"

        funcTester "funB(parA; parB) = parA + parB + y" "funB"
        valueTester "funB(y; x)" 39.0

        valueTester "funB(y; 1)" 37.0

    [<Fact>]
    let FuncsVariablesAndScope() =
      
        let controller = initController()

        let valueTester = bindController controller testValue
        let funcTester = bindController controller testNewFunction
        let variableTester = bindController (controller) testAssigment

        funcTester "funA(x; y) = x - y" "funA"
        variableTester "x = 10" 10.0 "x"        
        valueTester "funA(2, 1)" 1.0

        funcTester "funB(x; y) = x - y" "funB"
        valueTester "funB(3,1)" 2.0

    [<Fact(Skip="Not sure about enclosures yet.")>]
    let FuncsAndEnclosures() =
      
        let controller = initController()

        let valueTester = bindController controller testValue
        let funcTester = bindController controller testNewFunction
        let variableTester = bindController (controller) testAssigment

        variableTester "x = 10" 10.0 "x"        
        funcTester "funC(parA) = x - parA" "funC"
        valueTester "funC(7)" 3.0

        variableTester "x = 100" 100.0 "x"
        valueTester "funC(2.0)" -98.0


    [<Fact>]
    let FuncDeep() =
        let controller = initController()

        let valueTester = bindController controller testValue
        let funcTester = bindController controller testNewFunction

        funcTester "funC(x; y) = x + x * y" "funC"

        valueTester "funC(funC(1;2); 3)" 12.0
        valueTester "funC(funC(1; 3); funC(20; funC(1; 3)))" 404.0


         

