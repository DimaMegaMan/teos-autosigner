'Teos.Autosigner' is a code sample which allows to sign and submit the transactions, posted to TEOS API, for the predefined list of public address and private key pairs, by using TEOS API key, without user interaction

Disclaimer:
> Please note that this code only gives you an idea of how the autosigner should work. It should not be considered as a production-ready solution.

> You need to make sure, that the 'appsettings.json' file is properly protected from unintented use, as it contains sensitive information (private keys, teos api key).

How to run:

- Install .NET 6 SDK: https://dotnet.microsoft.com/en-us/download/dotnet/6.0

- Clone the repo and navigate to `Teos.Autosigner/src/Teos.Autosigner` folder:

  ```git clone git@github.com:CoreLedger-TEOS/teos-autosigner.git && cd Teos.Autosigner/src/Teos.Autosigner```

- Adjust `appsettings.json` file and save the changes:
```json5
{
  "SignerServiceOptions": {
    // array of public address and private key pairs, that will sign the transactions
    "Wallets": [
      {
        "PublicAddress": "public_address",
        "PrivateKey": "private_key"
      }
    ]
  },
  "TeosApiClientOptions": {
    // address of TEOS API endpoint
    "ApiUrl": "https://teos-dev2.dev.coreledger.net/odata/v0.9/",
    // TEOS API key of the customer
    "ApiKey": "teos_api_key"
  },
  "AutosignerWorkerOptions": {
    // how often to check for new unsigned transactions
    "Interval": "00:01:00",
    // whether the app should just log uncaught exceptions to console and proceed (true) or exit (false)
    "ContinueOnException": true
  }
}
```

- run the command to start the app:

  ```dotnet run```

- Create a Transaction with the configured public address, it will be signed and submitted with Teos.Autosigner. (Make sure the public address has enough ETH avaiable for signing the transaction)