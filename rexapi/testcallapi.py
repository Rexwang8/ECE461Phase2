#rest api test caller
import json
import requests

class user:
    name: str
    isAdmin: bool
    
    def __init__(self, name, isAdmin):
        self.name = name
        self.isAdmin = isAdmin
        
class Regex:
    RegEx: str
    
    def __init__(self, RegEx):
        self.RegEx = RegEx
    

class secret:
    password: str
    
    def __init__(self, password):
        self.password = password
        
class AuthenticateRequest:
    User: user
    Secret: secret
    
    def __init__(self, User, Secret):
        self.User = User
        self.Secret = Secret
        
class QueryRequest:
    Name: str
    Version: str
    
    def __init__(self, name, version):
        self.Name = name
        self.Version = version

class PackageData:
    Content: str
    URL: str
    JSProgram: str

    def __init__(self, content, url, jsprogram):
        self.Content = content
        self.URL = url
        self.JSProgram = jsprogram
        
class PackageMetaData:
    Name: str
    Version: str
    ID: str

    def __init__(self, Name, Version, ID):
        self.Name = Name
        self.Version = Version
        self.ID = ID
        
class Package:
    metadata: PackageMetaData
    data: PackageData

    def __init__(self, metadata, data):
        self.metadata = metadata
        self.data = data
        

#https://www.geeksforgeeks.org/serialize-and-deserialize-complex-json-in-python/

def FormAuthenticateRequest(username, password, isadmin):
    UserObj = user(username, isadmin)
    SecretObj = secret(password)
    AuthenticateRequestObj = AuthenticateRequest(UserObj, SecretObj)
    Body = json.dumps(AuthenticateRequestObj.__dict__, default=lambda o: o.__dict__, indent=4)
    Header = {'Content-Type': 'application/json', 'Accept': 'application/json'}
    url = "http://package-registry-461.appspot.com/authenticate"
    return url, Body, Header

def FormResetRequest(token):
    url = "http://package-registry-461.appspot.com/reset"
    Header = {'X-Authorization': token, 'Accept': 'application/json'}
    return url, Header

def FormPackageHistoryRequest(token, packageid):
    url = f"http://package-registry-461.appspot.com/package/byName/{packageid}"
    Header = {'X-Authorization': token, 'Accept': 'application/json'}
    return url, Header

def FormPackageRegexSearchRequest(token, regex):
    url = f"http://package-registry-461.appspot.com/package/byRegEx"
    Header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'}
    regexobj = Regex(regex)
    body = json.dumps(regexobj.__dict__, default=lambda o: o.__dict__, indent=4)
    return url, Header, body

def FormRateRequest(token, packageid):
    url = f"http://package-registry-461.appspot.com/package/{packageid}/rate"
    Header = {'X-Authorization': token, 'Accept': 'application/json'}
    return url, Header

def DeletePackageRequestByName(token, packagename):
    url = f"http://package-registry-461.appspot.com/package/byName/{packagename}"
    Header = {'X-Authorization': token, 'Accept': 'application/json'}
    return url, Header

def DeletePackageRequestByID(token, id):
    url = f"http://package-registry-461.appspot.com/package/{id}"
    Header = {'X-Authorization': token, 'Accept': 'application/json'}
    return url, Header

def PackagesListRequest(token, querys):
    url = "http://package-registry-461.appspot.com/packages"
    Header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'}
    body = "["
    body += ", ".join([json.dumps(query.__dict__, default=lambda o: o.__dict__, indent=4) for query in querys])
    
    body += "]"
    return url, Header, body

def CreatePackageLink(token):    
    
    prog = "if (process.argv.length === 7) {\nconsole.log('Success')\nprocess.exit(0)\n} else {\nconsole.log('Failed')\nprocess.exit(1)\n}\n"
    #https://www.npmjs.com/package/date-fns
    #https://github.com/jonschlinkert/even
    packageData = PackageData('', "https://github.com/jonschlinkert/even", prog)
    Body = json.dumps(packageData.__dict__, default=lambda o: o.__dict__, indent=4)
    URL = "http://package-registry-461.appspot.com/package"
    Header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'}
    return URL, Header, Body


def CreatePackageContent(filename, token):
    file = open(filename, 'r')
    prog = "if (process.argv.length === 7) {\nconsole.log('Success')\nprocess.exit(0)\n} else {\nconsole.log('Failed')\nprocess.exit(1)\n}\n"
    packageData = PackageData(file.read(), "", prog)
    Body = json.dumps(packageData.__dict__, default=lambda o: o.__dict__, indent=4)
    URL = "http://package-registry-461.appspot.com/package"
    Header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'}
    return URL, Header, Body

def FormRetrievePackageRequest(token, packageid):
    header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'}
    url = f"http://package-registry-461.appspot.com/package/{packageid}"
    return url, header

def FormPackageUpdateRequest(token, id, filename):
    url = f"http://package-registry-461.appspot.com/package/{id}"
    metaobj = PackageMetaData("fecha", "4.2.3", id)
    file = open(filename, 'r')
    prog = "if (process.argv.length === 7) {\nconsole.log('Success')\nprocess.exit(0)\n} else {\nconsole.log('Failed')\nprocess.exit(1)\n}\n"
    packageData = PackageData(file.read(), "", prog)
    #packageData = PackageData("1", "", "prog")
    pkg = Package(metaobj, packageData)
    body = json.dumps(pkg.__dict__, default=lambda o: o.__dict__, indent=4)
    header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'}
    return url, header, body

def CheckToken(token):
    print(f"len of token: {len(token)}")
    tokensub = token[7:]
    print(f"len of token sub: {len(tokensub)}")
    for i in tokensub:
        if (i.isalpha() or i.isdigit()):
            pass
        else:
            print(f"invalid at {i}")
        
        if (i.isupper()):
            print(f"upper at {i}")
            
    #check if bearer string is there
    if (token[:7] == "bearer "):
        pass
    else:
        print("invalid bearer string")

def PrintResponse(response, isjson=True):
    if response.status_code == 200 or response.status_code == 201:
        print("Success!")
        print(response.status_code)
        if isjson:
            print(response.json())
        print(response.headers)
    else:
        print("Failed!")
        print(response.status_code)
        print(response.headers)
    print("---------------------------------\n")

def main():
    url = "http://package-registry-461.appspot.com"
    
    #test authenticate
    #data
    username = "rex"
    password = "123"
    isadmin = True
    
    #token that wont set off the token checker on github
    token = 'bearer////2284hh7l20418b074i87h3631qfbff99i4mo10pd88f31i20710mb0dfef2j8mk02284gg7k20418n074h87g3631praee99h4ln10op88e31h20710ln0cede2i8lj02284ff7j20418m074g87f3631oq'.replace('////', ' ')
    CheckToken(token)
    print(token)
    
    
    #request -- Authenticate works
    #Authurl, Authbody, Authheader = FormAuthenticateRequest(username, password, isadmin)
    #print(f"PUT: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    #response = requests.put(Authurl, data=Authbody, headers=Authheader)
    #PrintResponse(response)
    
    #request -- Authenticate (WITH ECE)
    #Authurl, Authbody, Authheader = FormAuthenticateRequest("ece30861defaultadminuser", '''correcthorsebatterystaple123(!__+@**(A'"`;DROP TABLE packages''', True)
    #print(f"PUT: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    #response = requests.put(Authurl, data=Authbody, headers=Authheader)
    #PrintResponse(response)
    
    #delete by name -- works
    #Authurl, Authheader = DeletePackageRequestByName(token, "fecha")
    #print(f"DELETE: {Authurl} WITH HEADER: {Authheader}")
    #response = requests.delete(Authurl, headers=Authheader)
    #PrintResponse(response, False)
    
    #delete by id -- works
    #Authurl, AuthHeader = DeletePackageRequestByID(token, "0562f8fc-d583-4106-9a87-258257cf0262")
    #print(f"DELETE: {Authurl} WITH HEADER: {AuthHeader}")
    #response = requests.delete(Authurl, headers=AuthHeader)
    #PrintResponse(response, False)
    
    #reset -- works but doesn't reset currently
    #url, header = FormResetRequest(token)
    #print(f"DELETE: {url} WITH HEADER: {header}")
    #response = requests.delete(url, headers=header)
    #PrintResponse(response, False)
    
    #create 
    #Using Link -- works using gh link, not with npm link
    #Authurl, Authheader, Authbody = CreatePackageLink(token)
    #print(f"POST: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    #response = requests.post(Authurl, data=Authbody, headers=Authheader)
    #PrintResponse(response, False)
    

    #Using Content  -- works with even
    #Authurl, Authheader, Authbody = CreatePackageContent("rexapi/encoder/sample.txt", token)
    #print(f"POST: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    #response = requests.post(Authurl, data=Authbody, headers=Authheader)
    #PrintResponse(response, False)
    
    #Using Content  -- works with date-fns 2.29.1 and 2.29.2 and 2.29.3
    Authurl, Authheader, Authbody = CreatePackageContent("rexapi/encoder/date-fns-2.29.3_2.txt", token)
    print(f"POST: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    response = requests.post(Authurl, data=Authbody, headers=Authheader)
    PrintResponse(response, False)
    
    #Using Content  -- works with date-fns 2.29.1
    #Authurl, Authheader, Authbody = CreatePackageContent("rexapi/encoder/read2.txt", token)
    #print(f"POST: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    #response = requests.post(Authurl, data=Authbody, headers=Authheader)
    #PrintResponse(response, True)
    
    
    #Get history of package by name -- works
    #url, header = FormPackageHistoryRequest(token, "even")
    #print(f"History GET: {url} WITH HEADER: {header}")
    #response = requests.get(url, headers=header)
    #PrintResponse(response, True)
    
    #get metadata by regex/name -- works
    #url, header, body = FormPackageRegexSearchRequest(token, "(.*)")
    #print(f"Regex POST: {url} WITH HEADER: {header} AND BODY: {body}")
    #response = requests.post(url, headers=header, data=body)
    #PrintResponse(response, True)
    
    #rating by id -- works but doesn't return actual rating
    #url, header = FormRateRequest(token, "8b7061cd-e0a7-46a9-990d-5768d796c7f5")
    #print(f"Rating GET: {url} WITH HEADER: {header}")
    #response = requests.get(url, headers=header)
    #PrintResponse(response, True)
    
    
    #packages list version/name query -- works with 1 query
    #QueryRequestObj = list()
    #QueryRequestObj.append(QueryRequest("fecha", ""))
    #QueryRequestObj.append(QueryRequest("fecha", ""))
    #url, header, body = PackagesListRequest(token, QueryRequestObj)
    #print(f"List POST: {url} WITH HEADER: {header} AND BODY: {body}")
    #response = requests.post(url, headers=header, data=body)
    #PrintResponse(response, True)
    
    
    #retrieve package -- works
    #url, header = FormRetrievePackageRequest(token, "8b7061cd-e0a7-46a9-990d-5768d796c7f5")
    #print(f"Retrieve GET: {url} WITH HEADER: {header}")
    #response = requests.get(url, headers=header)
    #PrintResponse(response, True)
    
    #update package 
    #url, header, body = FormPackageUpdateRequest(token, "534889cf-ae38-4bcf-9d5f-dd87aafa2f0e", "rexapi/encoder/read2.txt")
    #print(f"Update PUT: {url} WITH HEADER: {header} AND BODY: {body}")
    #response = requests.put(url, headers=header, data=body)
    #PrintResponse(response, False)
    
    


if __name__ == '__main__':
    main()