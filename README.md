# RE4-DCT-TOOL
Extract and repack RE4 DCT files (RE4 UHD/PS4/NS)

**Translate from Portuguese Brazil**

Programa destinado a extrair e reempacotar arquivos .DCT;
<br> Ao extrair, será gerado um arquivo de extenção .idxdct e .txt2, eles serão usados para o repack;
<br> Nota: esses arquivos são encontrados na pasta "BIO4\text";

## Extract

Exemplo:
<br>RE4_DCT_TOOL_*.exe "ENGLISH_WIN32.dct"

! Vai gerar um arquivo de nome "ENGLISH_WIN32.idxdct" e outro de nome "ENGLISH_WIN32.txt2";

## Repack

Exemplo:
<br>RE4_DCT_TOOL_*.exe "ENGLISH_WIN32.idxdct"

! No arquivo .idxdct estão as informações necessárias para o repack;
<br>! No arquivo .txt2 tem o texto a ser editado;


## TXT2

! Esse é um arquivo UTF8; 
<br>! Nesse arquivo, cada linha é uma entry, o caractere @ representa a quebra de linha na entry;
<br>! Aviso: Não mude a quantidade de linhas.

**At.te: JADERLINK**
<br>2024-12-29