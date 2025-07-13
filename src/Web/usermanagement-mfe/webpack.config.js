const ModuleFederationPlugin = require("@module-federation/webpack");

module.exports = {
  mode: "development",
  plugins: [
    new ModuleFederationPlugin({
      name: "usermanagement-mfe",
      filename: "remoteEntry.js",
      exposes: {
        "./Module": "./src/app/usermanagement/usermanagement.module.ts"
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
