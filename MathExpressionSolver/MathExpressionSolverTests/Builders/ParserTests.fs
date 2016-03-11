namespace MathExpressionSolver.Tests

open Xunit
open FsUnit.Xunit

open System
open System.Collections.Generic
open MathExpressionSolver

open MathExpressionSolver.Tokens
open MathExpressionSolver.Controller
open MathExpressionSolver.Parser

open System.Linq
open System


module ParserTests =
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections

    let parseItemToString (item : ParsedItem) =
        "[" + item.Value + ":" + item.Type.ToString() + "]"

    let toStringAndConcate (parsedArray : ParsedItem[]) =
        Array.map parseItemToString parsedArray |> String.concat String.Empty

    let initParser () =
        new ExpressionParser();

    [<Fact>]
    let EmptyExpression() = 
        let parser = initParser ()
        parser.ParseExpression "" |> should equal Array.empty<ParsedItem>

    [<Fact>]
    let ExpressionNullExceptionTest() = 
        let parser = initParser ()
        (fun () -> parser.ParseExpression null |> ignore) |> should throw (typeof<ArgumentNullException>) //Must ignore because the should throw expects void delegate 

    [<Fact>]
    let SimpleTest() = 
        let parser = initParser ()
        parser.ParseExpression "2 + 3" |> should equal [|new ParsedItem("2", ParsedItemType.Value); new ParsedItem("+", ParsedItemType.Operator); new ParsedItem("3", ParsedItemType.Value)|]

    [<Fact>]
    let SequantialParses () =
        let parser = initParser ()
        let firstResult = parser.ParseExpression "2 + 3" 
        let secondResult = parser.ParseExpression "3 + 4"
        let thirdResult =  parser.ParseExpression "2 + 3" 
        
        should equal firstResult thirdResult

    [<Fact>]
    let WhiteSpaceTest () = 
        let parser = initParser ()
        parser.SkipWhiteSpace <- false
        parser.ParseExpression "2  + 3 " |> toStringAndConcate |> should equal "[2:Value][  :WhiteSpace][+:Operator][ :WhiteSpace][3:Value][ :WhiteSpace]"

        parser.SkipWhiteSpace <- true
        parser.ParseExpression "2  + 3 " |> toStringAndConcate |> should equal "[2:Value][+:Operator][3:Value]"

  
    [<Fact>]
    let TestNumbers () = 
        let parser = initParser ()
        parser.ParseExpression "+22.232 3" |> toStringAndConcate |> should equal "[+:Operator][22.232:Value][3:Value]"

    [<Fact>]
    let TestPositiveNumbers () = 
        let parser = initParser ()
        parser.ParseExpression "3+22.232 3" |> toStringAndConcate |> should equal "[3:Value][+:Operator][22.232:Value][3:Value]"

    [<Fact(Skip="Didn't work out parsing of negative numbers yet.")>]
    let TestNegativeNumbers () =
        let parser = initParser ()
        parser.ParseExpression "-22.232" |> toStringAndConcate |> should equal "[-22.232:Value]"
        parser.ParseExpression "+ -22.232" |> toStringAndConcate |> should equal "[+:Operator][-22.232:Value]"


    [<Fact>]
    let TestNames () = 
        let parser = initParser ()
        parser.ParseExpression "abc_de" |> toStringAndConcate |> should equal "[abc_de:Name]"
        parser.ParseExpression "2abc_de2" |> toStringAndConcate |> should equal "[2:Value][abc_de:Name][2:Value]"
        parser.ParseExpression "*abc_de*" |> toStringAndConcate |> should equal "[*:Operator][abc_de:Name][*:Operator]"
        parser.ParseExpression "ab;de" |> toStringAndConcate |> should equal "[ab:Name][;:Separator][de:Name]"
        parser.ParseExpression "ab2de" |> toStringAndConcate |> should equal "[ab:Name][2:Value][de:Name]"

    [<Fact>]
    let TestBrackets () = 
        let parser = initParser ()
        parser.ParseExpression "(2 )" |> toStringAndConcate |> should equal "[(:LBracket][2:Value][):RBracket]"
        parser.ParseExpression "[{2 )" |> toStringAndConcate |> should equal "[[:LBracket][{:LBracket][2:Value][):RBracket]"

    [<Fact>]
    let TestInvalidChar () = 
        let parser = initParser ()
        parser.ParseExpression "2\"'2" |> toStringAndConcate |> should equal "[2:Value][\":Invalid][':Invalid][2:Value]"