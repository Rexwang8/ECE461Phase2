#rest api test caller
import json
import requests

class user:
    name: str
    isAdmin: bool
    
    def __init__(self, name, isAdmin):
        self.name = name
        self.isAdmin = isAdmin

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

def main():
    url = "http://package-registry-461.appspot.com"
    
    #test authenticate
    #data
    username = "rex"
    password = "123"
    isadmin = True
    Authurl, Authbody, Authheader = FormAuthenticateRequest(username, password, isadmin)
    #bearer 2284hh7l20418b074i87h3631qfbff99i4mo10pd88f31i20710mb0dfef2j8mk02284gg7k20418n074h87g3631praee99h4ln10op88e31h20710ln0cede2i8lj02284ff7j20418m074g87f3631oq
    #request
    print(f"PUT: {Authurl} WITH BODY: {Authbody} AND HEADER: {Authheader}")
    response = requests.put(Authurl, data=Authbody, headers=Authheader)
    if response.status_code == 200:
        print("Success!")
    else:
        print("Failed!")
        
    #response
    print(response.json())
    #print headers
    print(response.headers)
    print("---------------------------------\n")


if __name__ == '__main__':
    main()