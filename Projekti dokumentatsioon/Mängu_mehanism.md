# Mängumehhaanika

## Üldine mängutsükkel

Mängu põhimehhanika põhineb järgmisel tsüklil:

1. Mängija liigub kaardil vaenlasi vältides
2. Mängija ründab vaenlasi
3. Vaenlaste tapmisel saab mängija kogemuspunkte
4. Taseme tõustes valib mängija uuenduse
5. Vaenlased muutuvad tugevamaks ja neid tekib rohkem
6. Tsükkel kordub kuni mängija surmani

Mängu eesmärk on ellu jääda võimalikult kaua.


## Mängija liikumine

- Mängija liigub 2D tasapinnal (ülaltvaates).
- Juhtimine toimub klaviatuuri abil (WASD või nooleklahvid).
- Mängija saab vabalt liikuda igas suunas.
- Liikumiskiirust saab uuendustega suurendada.


## Ründemehhanism

- Rünnak toimub automaatselt (või hiirega).
- Relv sihib lähimat vaenlast (või mängija ise sihib) ning tulistab kindla intervalliga.
- Relva omadused:
    - Kahju (damage)
    - Rünnakukiirus
    - Kuulide arv
    - Kuulide liikumiskiirus

Taseme tõustes saab neid omadusi parandada.


## Vaenlaste süsteem

- Vaenlased tekivad ajapõhiselt (või raundipõhiselt).
- Aja möödudes:
    - Vaenlaste arv suureneb
    - Vaenlaste elud suurenevad
    - Vaenlaste kiirus võib suureneda
- Erinevad vaenlase tüübid:
    - Tavaline (aeglane, madal elu)
    - Kiire (vähem elu, suurem kiirus)
    - Tugev (rohkem elu, aeglane)

Kui vaenlane puudutab mängijat, saab mängija kahju.


## Elude ja kahju süsteem

- Mängijal on kindel elude arv (HP).
- Iga kokkupõrge vaenlasega vähendab elusid.
- Kui elud jõuavad nulli, lõpeb mäng.
- Võimalikud uuendused võivad suurendada maksimaalset elude arvu või taastada tervist.


## Kogemus ja tasemed

- Iga tapetud vaenlane annab kogemuspunkte (XP).
- Kui XP riba täitub:
    - Kuvatakse 3 juhuslikku uuenduse valikut
    - Mängija valib ühe

XP nõue suureneb iga tasemega.


## Uuenduste süsteem

Uuendused võivad olla näiteks:

- +10% kahju
- +10% rünnakukiirus
- +1 projektiil
- +liikumiskiirus
- +maksimaalne elu

Uuendused on kumulatiivsed (efektid liituvad).


## Raskusastme kasv

Raskus kasvab aja jooksul:

- Spawnimise sagedus suureneb
- Vaenlaste statistika paraneb
- Võimalikud tugevamad vaenlased hilisemas faasis


## Skoreerimissüsteem

Skoor põhineb:

- Ellujäämisajal
- Tapetud vaenlaste arvul
- Saavutatud tasemel

Mängu lõpus kuvatakse lõppskoor.


# Kokkuvõte

Mängumehhanika keskmes on lihtne, kuid sõltuvust tekitav tsükkel:
liikumine → ründamine → XP saamine → uuenduste valimine → raskuse kasv.

Mängu on lihtne õppida, kuid ellujäämine muutub aja jooksul järjest keerulisemaks.