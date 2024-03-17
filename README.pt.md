# Sharper Save

 Um simples sistema de salvamento local com os recursos de ofuscação dos dados e verificação de integridade.

## Como funciona?

O sistema contém o Scriptable Object **SaveManager** que fornece os métodos públicos Save e Load, salvando e carregando os dados para o outro Scriptable Object **SaveContainer**, que armazena a classe serializável **SaveData**.

![Save Manager](/imgs/savemanager.png)
![Save Container](/imgs/savecontainer.png)

No **SaveManager** você pode definir se quer salvar com ou sem proteção dos dados. Com a proteção desativada, ele salvará apenas o arquivo de dados contendo o conteúdo em JSON, sendo possível uma modificação extremamente simples por parte do usuário.

![Json Save](/imgs/jsonsave.png)

Com a proteção ativada haverá uma geração da hash dos dados usando o algoritmo SHA-256 junto com um "sal" e um embaralhamento dos bytes dos dados com base em uma semente que é usada em um gerador de números aleatórios, em seguida, salvando os dados em binário usando o **BinaryWriter** do próprio C#. O "sal" e a semente são definidas por você.

![Bytes Save](/imgs/bytessave.png)
![Save Hash](/imgs/savehash.png)

O embaralhamento dos bytes dificulta a visualização e a reorganização dos dados originais e a geração de uma hash fará possível a verificação da integridade dos dados, e por a hash ter um "sal", se não for do conhecimento do usuário, uma injeção de uma nova hash pra tentar burlar a verificação não seria possível.

 Se convertermos os bytes embaralhados para texto, obtemos o seguinte resultado:
> a"6st0.atait:}taaourNa40n4i"Da5:D{"tat83oeo20bl0"a71te0:"Dm7"n2la0"1:trf,D9"1"g",4,

Mais detalhes de como funciona se encontra na [Wiki](https://github.com/DisassembledSharper/Sharper-Save/wiki) desse repositório.
