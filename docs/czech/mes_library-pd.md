## MES Knihovna

### Úvod
MES knihovna se skládá ze čtyř hlavních částí: parseru, tokenizéru, expression tree builderu a kontroleru. První tři komponenty jsou striktně oddělené a případně snadno nahraditelné jinou implementací. Do funkčního celku je propojuje až controller, který se současně stará také o interpretaci textového vstupu a udržování stavu uživatelsky definovaných funkcí / proměnných.

Parser a Controller je (do určité míry) pokryt testy (projekt MathExpressionSolverTests). V případě Parseru jde o unit testy, kontrolující pouze Parser. Pro Controller jde o integrační testy, které testují celou knihovnu dohromady. 

### Implementace kontroleru:

Třída `Controller<T>` zajišťuje propojení jednotlivých částí knihovny a interpretaci vstupu. Vzhledem k důrazu na modularitu a využití některých principů DI vyžaduje pro inicializaci externě dodat implementace `IParser`, `ITokenizer<T>` a `IExpTreeBuilder<T>`.

Veřejně poskytuje celou řadu metod pro zpracování příkazů. Od obecných (`ExecuteExpression`, respektive `ExecuteExpressionSafe`), které první pomocí regexů zjistí, zda jde o jednoduchý matematický výraz, přiřazení proměnné nebo dekladaci nové funkce, po specifické, které rovnou provedou vyhodnocení či jen postaví expression tree a vrátí kořenový `IToken<T>`.

Speciální zmínku si zaslouží jen funkce `SaveVariable` a `SaveFunction`. První zmiňovaná vyhodnotí výraz a následně jej uloží přímo do interního slovníku proměnných v `TokenFactory` aktuálního `ITokenizer`'u. Druhá pak k dekladaci nové uživatelské funkce využije přímo `TokenFactory` aktuálního `ITokenizer`'u, přičemž se předpokládá, že jde o instanci typu implementujícího rozhraní `ICustomFunctionsAwareTokenFactory<T>`.

Výsledky controller vrací ve struktuře `Result<T>`, která kromě hodnoty a typu provedeného výrazu (přiřazení, vyhodnocení, ...) může obsahovat také výjimku (v případě, že byla použita bezpečná verze metody) nebo dodatečná data ve formě řetězce, kupříkladu název proměnné, do které byla uložena nová hodnota.

### Implementace parseru:
Rozhraní `IParser` vyžaduje existenci jediné metody - ParseExpression, která přijímá řetězec a vrací pole `ParsedItem`, což je struktura obsahující vždy string a typ aktuální položky - `ParsedItemType` (jméno, hodnota, operátor, whitespace, ...). 

Samotná implementace parseru je  vcelku triviální stavový automat, který si jako stav udržuje typ posledního znaku. Co se týče typů znaků, tak malá a velká písmena velké abecedy plus podtržítko se počítají za název, čísla a desetiná tečka (nezávisle na kultuře) za číslo, čárka a středník za oddělovač a tařka cokoliv jiného, vyjma bílých znaků, za operátory.

### Tokeny:

Celá knihovna (od Parseru výše) je postavena na Tokenech. V první řadě na mutabilních `IFactorableToken<T>`'ech, které emituje Tokenizér a z kterých ExpTreeBuilder staví syntaktický strom, a následně nemutabilních `IToken<T>`'ech, které ExpTreeBuilder vrací jako kořen stromu.

Důležitým pricnipem za factorable tokeny je, že ještě nereprezentují hotové uzly stromu, ale pouze stavební materiál. V případě elementů s argumenty v závorce tak obsahují pole polí factorable tokenů, ze kterých se pro každý argument následně postaví a přiřadí vlastní expression tree.

Implementace výše zmíněých rozhraní reprezentují jednotlivé prvky syntaktického stromu, jako například uzel hodnoty, sčítání, nebo funkce sinus. 

### Implementace Tokenizéru:
Interface `ITokenizer<T>` vynucuje pouze jednu metodu `Tokenize` přijímající pole `ParsedItem` a vracející pole `IFactorableToken<T>` a veřejnou vlastnost TokenFactory umožňující dodat implementaci `ITokenFactory<T>`.

Dodaná implementace prochází dodané pole `ParsedItem`'ů a na základě typu aktuálního prvku, případně nejbližších sousedních, nechává `ITokenFactory<T>` vytvořit odpovídající `IFactorableToken<T>`.

V případě, že narazí na otevírací závorku, tak si poznačí, že je o úroveň níže a pokračuje dál. Následně, u uzavírající závorky zkompletuje pole `IFactorableToken<T>`ů vytvořených od poslední otevírající (tj. na stejné úrovni) a uloží je do Tokenu o úroveň výše, kterému dané argumenty odpovídají (buďto tokenu funkce nebo tokenu závorek). 

### Implementace expression tree builderu:

### Tvorba vlastních funkcí:
