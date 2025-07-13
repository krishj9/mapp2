const ModuleFederationPlugin = require("@module-federation/webpack");

module.exports = {
  mode: "development",
  plugins: [
    new ModuleFederationPlugin({
      name: "shell",
      remotes: {
        "planning-mfe": "http://localhost:4201/remoteEntry.js",
        "observations-mfe": "http://localhost:4202/remoteEntry.js", 
        "usermanagement-mfe": "http://localhost:4203/remoteEntry.js",
        "reports-mfe": "http://localhost:4204/remoteEntry.js"
      },
      shared: {
        "@angular/core": { singleton: true, strictVersion: true, requiredVersion: "auto" },
        "@angular/common": { singleton: true, strictVersion: true, requiredVersion: "auto" },
        "@angular/common/http": { singleton: true, strictVersion: true, requiredVersion: "auto" },
        "@angular/router": { singleton: true, strictVersion: true, requiredVersion: "auto" },
        "@angular/material": { singleton: true, strictVersion: true, requiredVersion: "auto" },
        "rxjs": { singleton: true, strictVersion: true, requiredVersion: "auto" }
      }
    })
  ]
};
