## MES Knihovna

### Úvod
MES knihovna se skládá ze čtyř hlavních částí: parseru, tokenizéru, expression tree builderu a kontroleru. První tři komponenty jsou striktně oddělené a případně snadno nahraditelné jinou implementací. Do funkčního celku je propojuje až controller, který se současně stará také o interpretaci textového vstupu a udržování stavu uživatelsky definovaných funkcí / proměnných.

Parser a Controller je (do určité míry) pokryt testy (projekt MathExpressionSolverTests). V případě Parseru jde o unit testy, kontrolující pouze Parser. Pro Controller jde o integrační testy, které testují celou knihovnu dohromady. 

### Implementace kontroleru:

Třída `Controller<T>` zajišťuje propojení jednotlivých částí knihovny a interpretaci vstupu. Vzhledem k důrazu na modularitu a využití některých principů DI vyžaduje pro inicializaci externě dodat implementace `IParser`, `ITokenizer<T>` a `IExpTreeBuilder<T>`.

Veřejně poskytuje celou řadu metod pro zpracování příkazů. Od obecných (`ExecuteExpression`, respektive `ExecuteExpressionSafe`), které první pomocí regexů zjistí, zda jde o jednoduchý matematický výraz, přiřazení proměnné nebo dekladaci nové funkce, po specifické, které rovnou provedou vyhodnocení či jen postaví expression tree a vrátí kořenový `IToken<T>`.

Speciální zmínku si zaslouží jen funkce `SaveVariable` a `SaveFunction`. První zmiňovaná vyhodnotí výraz a následně jej uloží přímo do interního slovníku proměnných v `TokenFactory` aktuálního `ITokenizer`'u. Druhá k dekladaci nové uživatelské funkce využije přímo `TokenFactory` aktuálního `ITokenizer`'u, přičemž se předpokládá, že jde o instanci typu implementujícího rozhraní `ICustomFunctionsAwareTokenFactory<T>`.

Výsledky controller vrací ve struktuře `Result<T>`, která kromě hodnoty a typu provedeného výrazu (přiřazení, vyhodnocení, ...) může obsahovat také výjimku (v případě, že byla použita bezpečná verze metody) nebo dodatečná data ve formě řetězce, kupříkladu název proměnné, do které byla uložena nová hodnota.

### Implementace parseru:
Rozhraní `IParser` vyžaduje existenci jediné metody - ParseExpression, která přijímá řetězec a vrací pole `ParsedItem`, což je struktura obsahující vždy string a typ aktuální položky - `ParsedItemType` (jméno, hodnota, operátor, whitespace, ...). 

Implementace parseru je vcelku triviální stavový automat, který si jako stav udržuje typ posledního znaku. Co se týče typů znaků, tak malá a velká písmena velké abecedy plus podtržítko se počítají za název, čísla a desetiná tečka (nezávisle na kultuře) za číslo, čárka a středník za oddělovač a tařka cokoliv jiného, vyjma bílých znaků, za operátory.

### Tokeny:

Celá knihovna (od Parseru výše) je postavena na Tokenech. V první řadě na mutabilních `IFactorableToken<T>`'ech, které emituje Tokenizér a z kterých ExpTreeBuilder staví syntaktický strom, a následně nemutabilních `IToken<T>`'ech, které ExpTreeBuilder vrací jako kořen stromu.

Důležitým pricnipem za factorable tokeny je, že ještě nereprezentují hotové uzly stromu, ale pouze stavební materiál. V případě elementů s argumenty v závorce tak obsahují pole polí factorable tokenů, ze kterých se pro každý argument následně postaví a přiřadí vlastní expression tree.

Implementace výše zmíněých rozhraní reprezentují jednotlivé prvky syntaktického stromu, jako například uzel hodnoty, sčítání, nebo funkce sinus. 

### Implementace Tokenizéru:
Interface `ITokenizer<T>` vynucuje pouze jednu metodu `Tokenize` přijímající pole `ParsedItem` a vracející pole `IFactorableToken<T>` a veřejnou vlastnost TokenFactory umožňující dodat implementaci `ITokenFactory<T>`.

Dodaná implementace prochází dodané pole `ParsedItem`'ů a na základě typu aktuálního prvku, případně nejbližších sousedních, nechává `ITokenFactory<T>` vytvořit odpovídající `IFactorableToken<T>`.

V případě, že Tokenizér narazí na otevírací závorku, tak si poznačí typ očekávaného tokenu s argumentem (tj. token závorky / funkce) a sejde o úroveň níže. Následně zas normálně vytváří tokeny, dokud nenarazí na závorku ukončovací. V ten moment zkompletuje všechny tokeny aktuální úrovně do pole, nechá `ITokenFactory<T>` vytvořit na základě dříve poznačené informace odpovídající Token (závorky / funkce), uloží něj pole argumentů a normálně pokračuje o úroveň výše.

### Implementace expression tree builderu:

Rozhraní `IExpTreeBuilder<T>` vyžaduje existenci jediné metody - CreateExpressionTree, která přijímá pole `IFactorableToken<T>` a vrací kořen vytvořeného expression stromu v podobě `IToken<T>`. 

Dodaná implementace je postavená na vlastním algoritmu optimalizovaném pro binární infixové operátory libovolné priority a, byť jen rozumně zanořené, funkce libovolného počtu proměnných.

Detailní popis je k nalezení ve funkci `placeCurrentToken` souboru `\Builders\ExpTreeBuilder.cs`.

Za zmínku stojí jen zpracování Tokenů s argumenty (funkce, závorky). V případě takových se na každý argument spustí ExpTreeBuilder rekurzivně a vytvoří vždy nezávislý expression tree, který následně uloží pomocí funkce `SetChild` na odpovídajícím tokenu.

### Deklarace vlastních funkcí:
Vlastní zmínku si zaslouží ještě dekladace a používání uživatelsky definovaných funkcí. Dekladace využívá speciální TokenFactory, konkrétně nějaké implementující rozhraní `ICustomFunctionsAwareTokenFactory<T>`. 

Do té předně vygeneruje nový Token representující uživatelskou funkci a uloží informaci o tom, jak se mají jmenovat jednotlivé argumenty, a následně nad ní spoustí normální generování expression tree. Pokud pak TokenFactory narazí na název, který by jindy mohl odpovídat jen proměnné, jehož název se shoduje s názvem nějakého argumentu, tak se vytvoří nový `ArgToken<T>`. Ten se jednak vrátí, jako každý TokenFactory vytvořený token, a druhak uloží do dříve uloženého Tokenu representujícího uživetelskou funkci.

Odkazy na argumenty z hlavního tokenu uživetelské funkce musejí existovat z důvodu nastavení jejich hodnoty pomocí funkce `SetChild` v ExpTreeBuilderu.

Při použití uživetelsky dekladované funkce pak dojde první k hluboké kopii expression a tree a následně k vrácení jeho kořenu. Drobný problém je ovšem s hlubokou kopií. Třebaže je totiž expression tree DAG, tak není - právě kvůli dvojitým odkazům na tokeny argumentů - stromem per se, objevuje se v něm diamontová závislost. 

Z toho důvodu si metoda `Clone` propaguje slovník instance-instance nahrazení. Při klonování se tedy v kořeni uživatelsky definované funkce vytvoří nové instance tokenů argumentů a uloží je do slovníku. Když pak rekurze klonování narazí na instance argumentů v samotném stromě výrazu, tak je nevytvoří znovu (což by způsobilo problém neb by oba odkazy mířily na jiné instance), ale použije ty již vytvořené a uložené do slovníku nahrazení.
