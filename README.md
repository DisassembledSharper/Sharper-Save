# Sharper Save

A simple local save system with hiding data and integrity check features.

[Portuguese Readme](/README.pt.md)

## How it works?

The system contains the **SaveManager** ScriptableObject that provides the Save and Load public methods, loading and saving data to another Scriptable Object **SaveContainer** that stores the **SaveData** serializable class.

![Save Manager](/imgs/savemanager.png)
![Save Container](/imgs/savecontainer.png)

In the **SaveManager** you can set if you want to save with or without data protection. When the protection is disabled, it will just save the data file containing the data in JSON, being possible a modification extremely simple by user.

![Json Save](/imgs/jsonsave.png)

When the protection is enabled, it will generates the data hash using the SHA-256 algorithm with a "salt" and a bytes shuffling based on a seed that will be used in the random number generator, then, saving the data in binary format using the **BinaryWriter** from C#. The "salt" and the seed are defined by you.

![Bytes Save](/imgs/bytessave.png)
![Save Hash](/imgs/savehash.png)

The bytes shuffling makes difficult the view and reorganization of the original data and the hash generation will make possible to check the data integrity, and because the hash has a "salt", if the user does not know it, a new hash injection to try to bypass the check won't be possible.

If we convert the shuffled bytes to text, we will have this result:
> a"6st0.atait:}taaourNa40n4i"Da5:D{"tat83oeo20bl0"a71te0:"Dm7"n2la0"1:trf,D9"1"g",4,

More details of how it works you can see this repository [Wiki](https://github.com/DisassembledSharper/Sharper-Save/wiki).
