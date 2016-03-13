namespace MathExpressionSolver.Tests
open System

module TestsUtils =

    let parseItemToString (item) =
        item.ToString()

    let toStringAndConcate (parsedArray : 'a[]) =
        Array.map parseItemToString parsedArray |> String.concat String.Empty
