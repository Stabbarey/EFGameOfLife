# GameOfLife


<h2>LaborationsSpecifikationen</h2>

<b>Reglerna för Game Of Life</b>
* En cell med färre än två grannar dör av ensamhet i nästa generation.
* En cell med fler än tre grannar dör av överbefolkning i nästa generation.
* En cell med två eller tre grannar överlever till nästa generation.
* I en tom cell som har exakt tre grannar kommer en ny cell att födas i nästa generation. Om cellen redan är bebodd sker ingen förändring.
* Alla födslar och dödsfall inträffar samtidigt. Det innebär till exempel att en cell som har fyra grannar, 
och alltså dör, ändå själv är granne och kan medverka till en annan cells död eller födelse i nästa generation.

<h3> Att göra</h3>

* Implementera spelet Game Of Life i valfritt GUI.Sök på nätet efter Game OfLife, det finns mycket information att hitta.

* Implementera möjlighet att spara ett spel (ALLA generationer)i en databas.
* Implementera funktionalitet, så att användarenkan ta bort sparade spel

* Implementera möjlighet att spela upp sparade spel. Exempelvis kan en uppspelning visas med en sekunds delay mellan generationerna.

* Använd i gruppen ett versionshanteringssystem. Vid redovisning förklaras hur ni jobbat med detta.


<h3>Exempel</h3>

<b>Exempel 1</b>

1. Användaren skriver in ett namn på spelet som ska sparas i databasen.
2. En matris initieras med några slumpmässiga levande celler.
3. När användaren klickar på en knapp, så skapas nästa generation. Detta kan göras hur många gånger användaren vill.
4. Spelet avslutas när användaren klickar på en knapp för att avsluta.5.Användaren kan senare öppna det sparade spelet och spela upp det.

<b>Exempel 2</b>

1. Användaren skriver in ett namn på spelet som ska sparas i databasen.
2. En matris initieras med ett förutbestämt mönster av levande celler
3. När användaren klickar på en knapp, så börjar nya generationer skapas med 1 sekunds delay.
4. När användaren klickar på en knapp för att avsluta, slutar spelet skapa nya generationer.
5. Användaren kan senare öppna det sparade spelet och spela upp det.

<h3>Redovisning</h3>

* Visa upp GOL-applikationen-Visa upp en inspelning av ett spel
* Visa upp en uppspelning av ett spel
* Förklara kortfattat hur ni löst problemet och hur ni använt Entity Framework
* Förklara hur ni använt er av versionhantering
* Maximalt 10min redovisningstid per grupp
