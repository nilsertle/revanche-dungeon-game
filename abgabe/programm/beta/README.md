Gruppe07 - Revanche

Steuerung:
Vom Hauptmenü kann mit den entsprechenden Buttons ein neues Spiel gestartet, ein Spielstand geladen, oder das Spiel verlassen werden. 

Die Kamera bewegt sich in Richtung des Mauszeigers, wenn dieser in die Nähe des Bildschirmrandes bewegt wird. Verbündete Einheiten (inkl. des Beschwörers) können 
mit einem Linksklick an- und wieder abgewählt werden, was visuell durch eine schwarze (abgewählt) bzw. blaue (ausgewählt) Umrandung angezeigt wird. 
Mit einem Rechtsklick auf eine freie Fläche innerhalb der Map (nicht auf Mauern) bewegen sich alle ausgewählten Einheiten zu diesem Punkt. 

Herumliegende "Seelen" werden von allen verbündeten Einheiten aufgesammmelt, welche sich über diese bewegen, und dem Vorrat des Beschwörers hinzugefügt. 
Tränke können nur vom Beschwörer per Tastendruck "f" benutzt werden. 

Der Beschwörer kann per Tastendruck "s" einen Feuerball in Richtung des Mauszeigers schießen. Dieser fügt jeder Einheit, die er trifft, Schaden zu. Der Feuerball verschwindet 
nach einer festgelegten Distanz. Der Beschwörer kann sich selbst treffen, wenn er schnell genug in seinen Feuerball rennt. 

Wenn der Beschwörer ausgewählt ist, kann dieser in den "Beschwörungs-Modus" wechseln:
Taste:
"q": beschwöre Imp
"w": beschwöre Skelett
"e": beschwöre magischen Setzling
"r": beschwöre Sturmwolke
"t": beschwöre Wasserelementar

Wir die selbe Taste noch einmal gedrückt, verlässt der Beschwörer den Beschwörungs-Modus. 
Welche Einheit beschworen werden wird, wird mit einen kleinen Icon über dem Kopf des Beschwörers angezeigt. Mit einem Linksklick auf ein feies Feld wird die ausgewählte
Einheit auf der Map platziert, und auch in der HUD hinzugefügt. Es kann ein Maximum an 5 Einheiten gleichzeitig aktiv sein. Stirbt eine Einheit, wird der Platz wieder frei.
Jede Einheit hat einen Balken, der ihre aktuellen Lebenspunkte anzeigt. Das Beschwören kostet Seelen. Zu Beginn hat der Beschwörer zu wenig Seelen, um eine Einheit zu beschwören,
es muss zuerst etwas Seele gesammelt werden. 

Über die Taste "d" kann der Debug-Modus aufgerufen werden. Damit werden die Hitboxen aller Einheite, Items und Projektile angezeigt, sowie das Raster der Map. Mit einem weiterem
Druck auf "d" wird der Debug-modus wieder verlassen. 

Mit "p" wird das Pausenmenü aufgerufen. Von dort kann das Spiel weitertgeführt, gespeichert, oder (zum Hauptmenü) verlassen werden. Dort finden sich auch die Optionen,
die im Moment lediglich wieder verlassen werden können. 

Momentan sind alle Eineheiten (mit Ausnahme des Beschwörers und des Erzfeindes (Die Figut, die über dem Altar in der Mitte des Startraumes steht)) Nahkämfer.
Kommt eine Einheit in die Nähe einer verfeindeten Einheit, bewegt dich die Einheit auf diese zu, bis sie in ihre Nahkampf-Reichweite kommt, und verursacht Schaden. 

Wird der Erzeind stirbt, wird bis zu 4x ein neues Level gestartet. 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Der Screenshot mainMenu_buttonMarkDown zeigt das Hauptmenü, und wie ein Button markiert wird, wenn die Maus über diesen hovert (Der Mauszeiger ist bei den Screenshots leider
nicht mit im Bild)

Der Screenshot level_hud_summoning zeigt eine typische Situation des Spiels: Der Spieler steht vor einem Blutaltar und beschwört mit dem Beschwörer einige Einheiten.
Über dem Kopf des Beschwörers ist die aktuell ausgewählte Einheit zu sehen. Die EInheiten werden in der Reihenfolge ihrer Beschwörung im Hud angezeigt.
Es liegen ein paar Items auf dem Boden verteilt. 

Der Sceenshot debug_battle zeigt die aktuelle Situation eines Kampfes: Alle Einheiten stehen auf einem Haufen, und verursachen Schaden an verfeindeten Einheiten. In Zunkunft wird
implementiert, dass die Einheiten miteinander kollidieren. Auf dem Screenshot ist auch zu sehen, wie der Spieler einen Feuerball verschießt. Auch sind durch den Debug-Modus
die Hitboxen der Spielobjekte und die Tiles der Map sichtbar. 