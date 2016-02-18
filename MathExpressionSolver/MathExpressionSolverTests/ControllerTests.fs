namespace MathExpressionSolver.Tests

open Xunit
open FsUnit.Xunit

open System
open System.Collections.Generic
open MathExpressionSolver

open MathExpressionSolver.Tokens
open MathExpressionSolver.Controller
open MathExpressionSolver.Parser

module ControllerIntegrationTests =

    let initController() = 
        let parser = new ExpressionParser()
        parser.SkipWhiteSpace <- true
        parser.SkipInvalidChars <- true

        let customVariables = new Dictionary<string, double>();
        let customFunctions = new Dictionary<string, IFactorableBracketsToken<double>>();

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

    let testInput (contr : Controller<double>) (expectedValue : string) (expression : string) = 
        let result = contr.ExecuteExpression expression
        result |> should equal expectedValue

        
    [<Fact>]
    let SimpleExpression() =
        let controller = initController()
        let result = controller.ReturnResult("2 + 3")
        result |> should (equalWithin 0.1) 5

    [<Fact>]
    let ComplexExpressions() =
        let testWithCurrInstance = testInput (initController())
        testWithCurrInstance "-107,306989780239" "3*(7+7)/2-2*6/7- &&&  (&6 +&9) *8-  (2+ 2/3*(6+exp  (2*7-6* 2 ) - 8)+(2>1))"
        testWithCurrInstance "-107,306989780239" "3*(7+7)/2-2*6/7-(6+9)*8-(2+2/3*(6+exp(2*7-6*2)-8)+(2>1))"

        testWithCurrInstance "1" "3 > 1"
        testWithCurrInstance "-5" "((2+1)-(3+1)*2)"

    [<Fact>]
    let Variables() =
        let testWithCurrInstance = testInput (initController())
        testWithCurrInstance "a = 2,5" "a=(3/6*5)"
        testWithCurrInstance "asdfsdf = 8" "asdfsdf=(5 + 3)"

        testWithCurrInstance "Pi = 3,4" "Pi = 3,4"
        testWithCurrInstance "7,6" "3 - Pi + 8"

        testWithCurrInstance "2,5" "a"
        testWithCurrInstance "8" "asdfsdf"

        testWithCurrInstance "13,3824939607035" "exp(a) + asdfsdf - 2*Pi"

    [<Fact>]
    let IfsSimple() =
        let testWithCurrInstance = testInput (initController())
        testWithCurrInstance "2" "if(1;2;3)" 
        testWithCurrInstance "3" "if(0;2;3)" 

    let prepareVariables (controller : Controller<double>) =
        controller.ExecuteExpression "a=(3/6*5)" |> ignore
        controller.ExecuteExpression "asdfsdf=(5 + 3)" |> ignore

        controller.ExecuteExpression "Pi = 3,4"|> ignore

    [<Fact>]
    let IfsAdv() =
        let controller = initController()
        prepareVariables controller
        let testWithCurrInstance = testInput controller

        testWithCurrInstance "13,3824939607035" "if((exp(100)> Pi)*2;exp(a) + asdfsdf - 2*Pi;2*3-asdfsdf)"
        testWithCurrInstance "1" "(if((exp(100)> Pi)*2;exp(a) + asdfsdf - 2*Pi;2*3-asdfsdf))*2>1" 

    [<Fact>]
    let FuncsSimple() =
        let testWithCurrInstance = testInput (initController())

        testWithCurrInstance "Function funA set." "funA(a;b) = a + b" 
        testWithCurrInstance "7" "funA(2;5)"

    [<Fact>]
    let FuncsAdv() =
        let contr = initController()
        contr.ExecuteExpression("Pi = 3,4") |> ignore
        let testWithCurrInstance = testInput contr

        testWithCurrInstance "Function funB set." "funB(height_in_m; diameter_in_cm) = Pi * ((diameter_in_cm / 100) * (diameter_in_cm / 100)) * height_in_m"

        testWithCurrInstance "0,000963490199635007" "funB(Pi; sin(20))"
        testWithCurrInstance "0,4352" "funB(20; 8)"


         

