# Security and Dependency Updates

This document describes the security and dependency updates made to the MCDSaveEdit project.

## Summary

The following NuGet packages have been updated to address security vulnerabilities and improve stability across the MCDSaveEdit and DungeonTools projects:

### Critical Security Updates

1. **System.Text.Json: 6.0.6 → 6.0.10**
   - **Projects Updated**: MCDSaveEdit, DungeonTools
   - **Severity**: High
   - **CVE**: CVE-2024-43485
   - **Issue**: Denial of Service vulnerability when deserializing input into models using `[ExtensionData]` property
   - **Fix**: Version 6.0.10 addresses this vulnerability
   - **Reference**: https://github.com/dotnet/announcements/issues/329

### Important Updates

2. **NLog: 5.0.4 → 5.3.4**
   - **Projects Updated**: MCDSaveEdit
   - **Reason**: Version 5.0.4 had no known vulnerabilities, but updating to 5.3.4 provides bug fixes and improvements
   - **Note**: Latest version is 6.0.7, but 5.3.4 is chosen for stability and minimal breaking changes
   - **Status**: No security vulnerabilities

3. **System.Diagnostics.DiagnosticSource: 6.0.0 → 6.0.1**
   - **Projects Updated**: MCDSaveEdit
   - **Reason**: Minor version update for bug fixes and improvements
   - **Status**: No security vulnerabilities

### Known Issues Not Fixed

4. **System.Net.Http 4.3.4**
   - **Status**: End-of-Life (EOL)
   - **Issue**: Microsoft no longer provides security patches for this version
   - **Why Not Updated**: This is a dependency of the .NET Framework 4.8 target. A full migration to .NET 6+ would be required to properly address this
   - **Mitigation**: The application uses .NET Framework 4.8 which includes System.Net.Http in the framework itself
   - **Recommendation**: Consider migrating to .NET 6+ in the future for better long-term support

### Already Secure

5. **Newtonsoft.Json 13.0.1**
   - **Status**: ✅ Already at secure version
   - **CVE**: CVE-2024-21907 (affects versions < 13.0.1)
   - **Note**: Version 13.0.1 includes the fix with default MaxDepth=128 to prevent DoS attacks

## Build Requirements

After pulling these changes, developers should:

1. Open the solution in Visual Studio 2022
2. Right-click the solution and select "Restore NuGet Packages"
3. Build the solution as normal

The project will automatically download the updated package versions from NuGet.

## Testing Recommendations

After updating:

1. ✅ Verify the application builds without errors
2. ✅ Test loading and saving character files
3. ✅ Test all main features (inventory editing, enchantments, etc.)
4. ✅ Ensure image loading from .pak files still works

## Future Recommendations

1. **Consider .NET 6+ Migration**: Moving to modern .NET would:
   - Eliminate the System.Net.Http EOL dependency
   - Provide better performance
   - Enable cross-platform support (already partially done with Steam Deck support)
   - Provide long-term security updates

2. **Regular Dependency Updates**: Schedule regular reviews (quarterly) of NuGet packages for security updates

3. **Automated Security Scanning**: Consider integrating tools like:
   - GitHub Dependabot
   - Snyk
   - OWASP Dependency-Check

## References

- [CVE-2024-43485 - System.Text.Json DoS](https://github.com/dotnet/announcements/issues/329)
- [CVE-2024-21907 - Newtonsoft.Json DoS](https://nvd.nist.gov/vuln/detail/CVE-2024-21907)
- [NLog Release Notes](https://github.com/NLog/NLog/releases)
- [.NET 6.0 Security Updates](https://github.com/dotnet/core/blob/main/release-notes/6.0/cve.md)
