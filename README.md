# CallStatsClient 

## CallStatsClient project:

* UWP sample app, sends test data.
* Uses CallStatsLib project to make requests to REST API. 
* Uses jose-jwt nuget to generate JWT token required for authentication.

## CallStatsLib project:

* .NET Standard 2.0 library. 
* Consists of send request method and methods for connecting endpoins and right json data.
* Uses Newtonsoft.Json to serialize/deserialize data.

## Authentication steps:

* Set App ID: copy from your application settings to CallStatsClient/Config.cs 
`localSettings.Values["appID"]`

* Set `secret string` in CallStatsClient/Config.cs 
`localSettings.Values["secret"]`

* Open Command Prompt: 
`set RANDFILE=.rnd` 

* Generate private key: 
`openssl ecparam -genkey -name secp256k1 -noout -out privatekey.pem`

* Generate public key: 
`openssl ec -in privatekey.pem -pubout -out pubkey.pem`

* Copy public key to your application settings.

* Set key id: copy from your application settings to CallStatsClient/Config.cs 
`localSettings.Values["keyID"]` 

* Create certificate: 
`openssl req -new -x509 -days 1826 -key privatekey.pem -out certificate.crt`

* Create .p12 certificate, use `secret string` for password: 
`openssl pkcs12 -export -out ecc-key.p12 -inkey privatekey.pem -in certificate.crt`

* Copy .p12 certificate to CallStatsClient `ecc-key.p12` file

