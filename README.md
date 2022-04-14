# pdfORjpg-sig-analysis app

A CLI program which can analyze a folder directory and all of its embedded subdirectories(recursively) for possible pdf and jpg files. Then adds those files' paths
to an output csv table, also records the original MD5 hash(used to detect file tampering) of the files. It determines whether a file is a pdf or jpg or neither via 
a file's signature type which can be found by opening a file in a byte array format and checking the first index(index 0).

Proof of functionality:
![image](https://user-images.githubusercontent.com/69401254/163437792-5fdd5c84-d8b3-461c-80e6-41b14e2f86ef.png)
