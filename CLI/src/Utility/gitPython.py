""" from git import Repo    
import os

class pythonGit:
    def pyClone(url, path):
        try:
            Repo.clone_from(url, path)
        except:
            pass
        
#download test repo
if __name__ == "__main__":
    f = open("test.txt", "w")
    f.write("wtf")
 """
f = open("test.txt", "w")
f.write("wtf")

