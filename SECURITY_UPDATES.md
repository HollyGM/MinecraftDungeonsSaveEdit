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

> **Do not downgrade System.Text.Json below 6.0.10.** Since the data-integrity fixes described in
> `ANALISE_ATUALIZACAO.md`, the save file models use `[JsonExtensionData]` to keep fields the editor does
> not know about. CVE-2024-43485 targets exactly that feature, so the patched version is now required.

2. **SkiaSharp: 2.88.3 → 2.88.9** (also `SkiaSharp.NativeAssets.Win32` and `SkiaSharp.NativeAssets.macOS`)
   - **Projects Updated**: MCDSaveEdit, PakReader
   - **Severity**: High
   - **Advisory**: [GHSA-j7hp-h8jx-5ppr](https://github.com/advisories/GHSA-j7hp-h8jx-5ppr) (libwebp heap buffer overflow, CVE-2023-4863), fixed in 2.88.6
   - **Note**: The vulnerable code is in the native `libSkiaSharp.dll`, which Costura embeds from
     `MCDSaveEdit/Costura32` and `MCDSaveEdit/Costura64`. Those files were replaced with the ones from the
     signed `SkiaSharp.NativeAssets.Win32` 2.88.9 package (`runtimes/win-x86` and `runtimes/win-x64`), so updating
     the package reference alone would not have been enough. Keep both in sync on future updates.

### Important Updates

3. **NLog: 5.0.4 → 5.3.4**
   - **Projects Updated**: MCDSaveEdit
   - **Reason**: Version 5.0.4 had no known vulnerabilities, but updating to 5.3.4 provides bug fixes and improvements
   - **Note**: Latest version is 6.0.7, but 5.3.4 is chosen for stability and minimal breaking changes
   - **Status**: No security vulnerabilities

4. **System.Diagnostics.DiagnosticSource: 6.0.0 → 6.0.1**
   - **Projects Updated**: MCDSaveEdit
   - **Reason**: Minor version update for bug fixes and improvements
   - **Status**: No security vulnerabilities

### Known Issues Not Fixed

5. **DotNetZip 1.16.0** (PakReader)
   - **Advisory**: [GHSA-xhg6-9j5j-w4vf](https://github.com/advisories/GHSA-xhg6-9j5j-w4vf) (directory traversal when extracting zip archives, CVE-2024-48510)
   - **Why Not Updated**: DotNetZip is deprecated and no patched version exists
   - **Exposure**: the advisory concerns extracting zip archives to disk; this application only reads the game's own `.pak` files
   - **Recommendation**: replace it in the PakReader submodule with `System.IO.Compression` (zlib/deflate streams are available in the framework)

6. **System.Net.Http 4.3.4**
   - **Status**: End-of-Life (EOL)
   - **Issue**: Microsoft no longer provides security patches for this version
   - **Why Not Updated**: This is a dependency of the .NET Framework 4.8 target. A full migration to .NET 6+ would be required to properly address this
   - **Mitigation**: The application uses .NET Framework 4.8 which includes System.Net.Http in the framework itself
   - **Recommendation**: Consider migrating to .NET 6+ in the future for better long-term support

### Already Secure

7. **Newtonsoft.Json 13.0.1**
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
