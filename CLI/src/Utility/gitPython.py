import os
import sys
from git import Repo  

class pythonGit:
    def pyClone(url, path):
        print(f"workingdir {os.getcwd()}")
        print(f"path {path}")
        try:
            print("um")
            Repo.clone_from(url, path)
        except:
            print("hello")
            pass

if __name__ == "__main__":
    folderName = sys.argv[1]
    cwd = os.getcwd()
    srcpath = os.path.dirname(cwd)
    modulespath = os.path.join(os.path.dirname(srcpath), "modules")
    print(f"modulespath {modulespath}")
    os.makedirs(modulespath, exist_ok=True)
    pathforfolder = os.path.join(modulespath, folderName)
    
    pythonGit.pyClone(sys.argv[2], pathforfolder)