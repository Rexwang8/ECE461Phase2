import os

import base64

def decode(filename):
    file = open(f"{filename}.txt", 'rb')
    with open(f"{filename}.zip", "wb") as fh:
        fh.write(base64.decodebytes(file.read()))
    file.close()
def encode(filename):
    file = open(f"{filename}.zip", 'rb')
    with open(f"{filename}_2.txt", "wb") as fh:
        fh.write(base64.encodebytes(file.read()))
    file.close()
    
def compare(filename):
    file1 = open(f"{filename}.txt", 'rb')
    file2 = open(f"{filename}_2.txt", 'rb')
    if file1.read() == file2.read():
        print("True")
    else:
        print("False")
    file1.close()
    file2.close()
    
def trimspace(filename):
    f = open(f"{filename}.txt", 'r')
    data = f.read()
    data = data.replace(" ", "").replace("\n", "")
    f.close()
    f = open(f"{filename}.txt", 'w')
    f.write(data)
    f.close()
    
if __name__ == '__main__':
   # decode("sample")
    #encode("sample")
    #trimspace("sample_2")
    #compare("sample")
    pwd = os.getcwd()
    print(pwd)
    #encode("rexapi/encoder/date-fns-2.29.3")
    trimspace("rexapi/encoder/read")
    decode("rexapi/encoder/read")
    #compare("rexapi/encoder/date-fns-2.29.3")