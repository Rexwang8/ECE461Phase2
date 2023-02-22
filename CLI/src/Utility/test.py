import os
import sys
from git import Repo  

class pythonGit:
    def pyClone(url, path):
        try:
            print("um")
            Repo.clone_from(url, path)
        except:
            print("hello")
            pass

if __name__ == "__main__":
    folderName = sys.argv[1]
    repoUrl = sys.argv[2]
    path = os.path.join("../modules", folderName)
    os.makedirs(path, exist_ok=True)
   
    pythonGit.pyClone("https://github.com/nullivex/nodist", os.getcwd() + "/../../modules/" + folderName)