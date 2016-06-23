## MES Server klient

### Úvod
V rámci solution MES je k dispozici také ukázková integrace knihovny v plně asynchroním socketovém serveru (projekt MathExpressionSolverSocketSever) a základní implementace tenkých klientů (projekt MESSocketClient).

### Implementace serveru - MathExpressionSolverSocketSever
Server je implemetován pomocí dvou tříd, `MESServer` - starající se o správu samotného serveru (ukončování, ...) a navazování spojení s novými klienty, a `ClientState` - která řeší samotnou komunikaci s jednotlivým klientem, počítání a udržování stavu. 

`MESServer` obsahuje zejména metodu `Work`, která spouští dvě důležité smyčky. První (metoda `readCommands`) neustále čte z konzole a čeká na příkaz `--bye`, aby následně aktivovala `CancellationToken`, zavřela skrze něj všechna spojení a ukončila server. A druhou (metody `doListening` a `listen`), která poslouchá na portu a asynchroně čeká na nové `TcpClient`'ty. Ke každému novému klientu následně vytvoří `ClientState` a asynchroně na něm spustí metodu `Talk` (ne-`await`'uje ji).

`ClientState` si drží dvě důležité instance, `TcpClient` representující spojení s klientem a `Controller<double>` z MES odpovídající stavu kalkulačky (uživatelské funkce, proměnné, ...).

Komunikace s klientem probíhá plně asynchroně nad `StreamReader`'em, respektive `StreamWriter`'em, vytvořeným z `NetworkStream`'u získáného z `TcpClient`'a. Všechny operace, které mají potenciál čekat, jsou jištěné `CancellationToken`'em. Konkrétně z něho vytvořeným Taskem (vždycky se čeká na `.WhenAny` a kontroluje se, jestli neskončil Task odpovídající Tokenu), takže případné ukončení serveru je prakticky okamžité.

Každý klient je navíc jištěn ještě timeoutem. Pokud čtení z klienta trvá déle než xyz sekund, tak je spojení automaticky přerušeno a klient zahozen.


Samotné provádění operace nad `Controller<double>`'em je synchroní, neb jeho explicitní odsunutí do dalšího vlákna (nebo i jen samostatného Tasku) by ničemu nepomohlo. Zpracování jednotlivých příkazů jednoho klienta musí být totiž stejně sériové.

### Implementace klientů - MESSocketClient
Implementace tenkých klientů je opravdu základní. V podstatě jde jen o plně synchroní echo klienty, kteří se na začátku přes socket připojí k serverové aplikaci, následně přeposílají vše napsané do terminálu a naopak zobrazují příchozí text. 

Za zmínku stojí snad jen použití obalové třídy `TcpClient` namísto čistých socketů a komunikace přes `StreamReader`, respektive `StreamWriter`, vytvořený nad vyzískaným `NetworkStream`'em.