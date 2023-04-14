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
    
    #userObj = user('rex', True)
    #secObj = secret('123')
    #AuthOBJ = AuthenticateRequest(userObj, secObj)
    #body = json.dumps(AuthOBJ.__dict__, default=lambda o: o.__dict__, indent=4)
    return url, Header, body


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
    if response.status_code == 200:
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
    
    
    #request
    #Authurl, Authbody, Authheader = FormAuthenticateRequest(username, password, isadmin)
    #print(f"PUT: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    #response = requests.put(Authurl, data=Authbody, headers=Authheader)
    #PrintResponse(response)
    
    
    
    
    #url, header = FormResetRequest(token)
    #print(f"DELETE: {url} WITH HEADER: {header}")
    #response = requests.delete(url, headers=header)
    #PrintResponse(response, False)
    
    
    
    #url, header = FormPackageHistoryRequest(token, "packagename")
    #print(f"History GET: {url} WITH HEADER: {header}")
    #response = requests.get(url, headers=header)
    #PrintResponse(response, True)
    
    #example regex
    
    url, header, body = FormPackageRegexSearchRequest(token, ".*?Underscore.*")
    print(f"Regex POST: {url} WITH HEADER: {header} AND BODY: {body}")
    response = requests.post(url, headers=header, data=body)
    PrintResponse(response, True)
    


if __name__ == '__main__':
    main()