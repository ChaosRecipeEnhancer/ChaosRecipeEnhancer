using ChaosRecipeEnhancer.UI.Configuration;
using System.Configuration;
using System.Xml.Linq;

namespace ChaosRecipeEnhancer.UI.Tests.Configuration;

/// <summary>
/// Tests for the legacy settings migration logic in <see cref="SettingsConfiguration"/>.
/// Covers three internal methods:
///   - ParseUserConfigXml: XML parsing of user.config files
///   - ApplyLegacyValues: type conversion and filtering of parsed values
///   - FindBestLegacyConfig: filesystem search for the best legacy config file
/// </summary>
public class SettingsConfigurationTests
{
    #region ParseUserConfigXml

    [Fact]
    public void ParseUserConfigXml_GivenValidConfig_ReturnsAllSettings()
    {
        // Arrange — a realistic user.config with multiple setting types
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""LeagueName"" serializeAs=""String"">
                    <value>Settlers</value>
                  </setting>
                  <setting name=""FullSetThreshold"" serializeAs=""String"">
                    <value>5</value>
                  </setting>
                  <setting name=""SoundEnabled"" serializeAs=""String"">
                    <value>True</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().HaveCount(3);
        result["LeagueName"].Should().Be("Settlers");
        result["FullSetThreshold"].Should().Be("5");
        result["SoundEnabled"].Should().Be("True");
    }

    [Fact]
    public void ParseUserConfigXml_GivenEmptyStringValue_IncludesIt()
    {
        // Arrange — empty string is a valid value (e.g., PathOfExileAccountName defaults to "")
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""PathOfExileAccountName"" serializeAs=""String"">
                    <value></value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert — empty string should be preserved, not skipped
        result.Should().ContainKey("PathOfExileAccountName");
        result["PathOfExileAccountName"].Should().BeEmpty();
    }

    [Fact]
    public void ParseUserConfigXml_GivenSelfClosingValueElement_IncludesEmptyString()
    {
        // Arrange — some .NET serializations produce <value /> instead of <value></value>
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""StashTabPrefix"" serializeAs=""String"">
                    <value />
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().ContainKey("StashTabPrefix");
        result["StashTabPrefix"].Should().BeEmpty();
    }

    [Fact]
    public void ParseUserConfigXml_GivenMissingSettingsSection_ReturnsEmptyDictionary()
    {
        // Arrange — XML is valid but doesn't contain our settings section
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <SomeOtherApp.Properties.Settings>
                  <setting name=""Foo"" serializeAs=""String"">
                    <value>Bar</value>
                  </setting>
                </SomeOtherApp.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseUserConfigXml_GivenEmptySettingsSection_ReturnsEmptyDictionary()
    {
        // Arrange — our settings section exists but has no <setting> children
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseUserConfigXml_GivenSettingWithNoNameAttribute_SkipsIt()
    {
        // Arrange — malformed setting entry missing the name attribute
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting serializeAs=""String"">
                    <value>Orphaned</value>
                  </setting>
                  <setting name=""LeagueName"" serializeAs=""String"">
                    <value>Standard</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert — only the valid setting should be present
        result.Should().HaveCount(1);
        result.Should().ContainKey("LeagueName");
    }

    [Fact]
    public void ParseUserConfigXml_GivenSettingWithEmptyNameAttribute_SkipsIt()
    {
        // Arrange
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name="""" serializeAs=""String"">
                    <value>Bad</value>
                  </setting>
                  <setting name=""SoundEnabled"" serializeAs=""String"">
                    <value>False</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainKey("SoundEnabled");
    }

    [Fact]
    public void ParseUserConfigXml_GivenSettingWithNoValueElement_SkipsIt()
    {
        // Arrange — setting element has name but no <value> child
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""MissingValue"" serializeAs=""String"">
                  </setting>
                  <setting name=""GoodSetting"" serializeAs=""String"">
                    <value>OK</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainKey("GoodSetting");
    }

    [Fact]
    public void ParseUserConfigXml_GivenDuplicateNames_LastOneWins()
    {
        // Arrange — shouldn't happen in practice, but verify deterministic behavior
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""LeagueName"" serializeAs=""String"">
                    <value>First</value>
                  </setting>
                  <setting name=""LeagueName"" serializeAs=""String"">
                    <value>Second</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert — dictionary assignment means last one wins
        result["LeagueName"].Should().Be("Second");
    }

    [Fact]
    public void ParseUserConfigXml_GivenFullRealisticConfig_ParsesAllSettingTypes()
    {
        // Arrange — a config with string, int, bool, float, double, and DateTime values
        // as they'd actually appear in a v3.27.1000 user.config
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""LeagueName"" serializeAs=""String"">
                    <value>Settlers</value>
                  </setting>
                  <setting name=""FullSetThreshold"" serializeAs=""String"">
                    <value>3</value>
                  </setting>
                  <setting name=""AutoFetchOnRezoneEnabled"" serializeAs=""String"">
                    <value>True</value>
                  </setting>
                  <setting name=""StashTabOverlayOpacity"" serializeAs=""String"">
                    <value>0.85</value>
                  </setting>
                  <setting name=""StashTabOverlayHeight"" serializeAs=""String"">
                    <value>720</value>
                  </setting>
                  <setting name=""StashTabOverlayHighlightColor"" serializeAs=""String"">
                    <value>#96F90000</value>
                  </setting>
                  <setting name=""SoundLevel"" serializeAs=""String"">
                    <value>0.75</value>
                  </setting>
                  <setting name=""LootFilterStylesRingTextFontSize"" serializeAs=""String"">
                    <value>42</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().HaveCount(8);
        result["LeagueName"].Should().Be("Settlers");
        result["FullSetThreshold"].Should().Be("3");
        result["AutoFetchOnRezoneEnabled"].Should().Be("True");
        result["StashTabOverlayOpacity"].Should().Be("0.85");
        result["StashTabOverlayHeight"].Should().Be("720");
        result["StashTabOverlayHighlightColor"].Should().Be("#96F90000");
        result["SoundLevel"].Should().Be("0.75");
        result["LootFilterStylesRingTextFontSize"].Should().Be("42");
    }

    [Fact]
    public void ParseUserConfigXml_GivenMinimalDocument_ReturnsEmptyDictionary()
    {
        // Arrange — completely empty configuration
        var doc = XDocument.Parse("<configuration />");

        // Act
        var result = SettingsConfiguration.ParseUserConfigXml(doc);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region ApplyLegacyValues

    /// <summary>
    /// Helper to build a <see cref="SettingsPropertyCollection"/> for testing.
    /// Creates typed properties so the type converter logic is exercised.
    /// </summary>
    private static SettingsPropertyCollection BuildProperties(params (string name, Type type)[] definitions)
    {
        var collection = new SettingsPropertyCollection();
        foreach (var (name, type) in definitions)
        {
            collection.Add(new SettingsProperty(name) { PropertyType = type });
        }
        return collection;
    }

    [Fact]
    public void ApplyLegacyValues_GivenMatchingStringProperty_ImportsIt()
    {
        // Arrange
        var legacyValues = new Dictionary<string, string> { { "LeagueName", "Settlers" } };
        var properties = BuildProperties(("LeagueName", typeof(string)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(1);
        applied["LeagueName"].Should().Be("Settlers");
    }

    [Fact]
    public void ApplyLegacyValues_GivenMatchingIntProperty_ConvertsAndImports()
    {
        // Arrange
        var legacyValues = new Dictionary<string, string> { { "FullSetThreshold", "5" } };
        var properties = BuildProperties(("FullSetThreshold", typeof(int)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(1);
        applied["FullSetThreshold"].Should().Be(5);
    }

    [Fact]
    public void ApplyLegacyValues_GivenMatchingBoolProperty_ConvertsAndImports()
    {
        // Arrange
        var legacyValues = new Dictionary<string, string> { { "SoundEnabled", "True" } };
        var properties = BuildProperties(("SoundEnabled", typeof(bool)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(1);
        applied["SoundEnabled"].Should().Be(true);
    }

    [Fact]
    public void ApplyLegacyValues_GivenMatchingDoubleProperty_ConvertsAndImports()
    {
        // Arrange
        var legacyValues = new Dictionary<string, string> { { "SoundLevel", "0.75" } };
        var properties = BuildProperties(("SoundLevel", typeof(double)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(1);
        applied["SoundLevel"].Should().Be(0.75);
    }

    [Fact]
    public void ApplyLegacyValues_GivenMatchingSingleProperty_ConvertsAndImports()
    {
        // Arrange — StashTabOverlayOpacity is System.Single (float)
        var legacyValues = new Dictionary<string, string> { { "StashTabOverlayOpacity", "0.85" } };
        var properties = BuildProperties(("StashTabOverlayOpacity", typeof(float)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(1);
        ((float)applied["StashTabOverlayOpacity"]).Should().BeApproximately(0.85f, 0.001f);
    }

    [Fact]
    public void ApplyLegacyValues_GivenSkippedAuthToken_DoesNotImport()
    {
        // Arrange — PathOfExileApiAuthToken is in the skip list
        var legacyValues = new Dictionary<string, string>
        {
            { "PathOfExileApiAuthToken", "secret-token-value" },
            { "LeagueName", "Standard" },
        };
        var properties = BuildProperties(
            ("PathOfExileApiAuthToken", typeof(string)),
            ("LeagueName", typeof(string)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert — only LeagueName should be imported, not the auth token
        count.Should().Be(1);
        applied.Should().ContainKey("LeagueName");
        applied.Should().NotContainKey("PathOfExileApiAuthToken");
    }

    [Fact]
    public void ApplyLegacyValues_GivenAllSkippedSettings_NoneAreImported()
    {
        // Arrange — every value in the legacy config is on the skip list
        var legacyValues = new Dictionary<string, string>
        {
            { "PathOfExileApiAuthToken", "token" },
            { "PathOfExileApiAuthTokenExpiration", "2025-01-01" },
            { "LegacyAuthSessionId", "session-id" },
            { "UpgradeSettingsAfterUpdate", "True" },
            { "LegacySettingsMigrated", "False" },
        };
        var properties = BuildProperties(
            ("PathOfExileApiAuthToken", typeof(string)),
            ("PathOfExileApiAuthTokenExpiration", typeof(DateTime)),
            ("LegacyAuthSessionId", typeof(string)),
            ("UpgradeSettingsAfterUpdate", typeof(bool)),
            ("LegacySettingsMigrated", typeof(bool)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(0);
        applied.Should().BeEmpty();
    }

    [Fact]
    public void ApplyLegacyValues_GivenPropertyNotInLegacy_SkipsIt()
    {
        // Arrange — current schema has a property that didn't exist in the old version
        var legacyValues = new Dictionary<string, string> { { "LeagueName", "Standard" } };
        var properties = BuildProperties(
            ("LeagueName", typeof(string)),
            ("BrandNewSetting", typeof(string)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(1);
        applied.Should().NotContainKey("BrandNewSetting");
    }

    [Fact]
    public void ApplyLegacyValues_GivenInvalidTypeConversion_SkipsAndContinues()
    {
        // Arrange — "not-a-number" can't convert to int, but the string one should still work
        var legacyValues = new Dictionary<string, string>
        {
            { "FullSetThreshold", "not-a-number" },
            { "LeagueName", "Standard" },
        };
        var properties = BuildProperties(
            ("FullSetThreshold", typeof(int)),
            ("LeagueName", typeof(string)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert — partial migration: the valid one succeeds, the bad one is skipped
        count.Should().Be(1);
        applied.Should().ContainKey("LeagueName");
        applied.Should().NotContainKey("FullSetThreshold");
    }

    [Fact]
    public void ApplyLegacyValues_GivenEmptyLegacyValues_ReturnsZero()
    {
        // Arrange
        var legacyValues = new Dictionary<string, string>();
        var properties = BuildProperties(("LeagueName", typeof(string)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(0);
        applied.Should().BeEmpty();
    }

    [Fact]
    public void ApplyLegacyValues_GivenEmptyProperties_ReturnsZero()
    {
        // Arrange
        var legacyValues = new Dictionary<string, string> { { "LeagueName", "Standard" } };
        var properties = new SettingsPropertyCollection();
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public void ApplyLegacyValues_GivenMultipleMixedTypes_ImportsAllValid()
    {
        // Arrange — simulates a realistic migration with multiple types
        var legacyValues = new Dictionary<string, string>
        {
            { "LeagueName", "Settlers" },
            { "FullSetThreshold", "3" },
            { "SoundEnabled", "True" },
            { "SoundLevel", "0.75" },
            { "StashTabOverlayOpacity", "0.9" },
            { "StashTabOverlayHeight", "720" },
            { "LootFilterManipulationEnabled", "False" },
        };
        var properties = BuildProperties(
            ("LeagueName", typeof(string)),
            ("FullSetThreshold", typeof(int)),
            ("SoundEnabled", typeof(bool)),
            ("SoundLevel", typeof(double)),
            ("StashTabOverlayOpacity", typeof(float)),
            ("StashTabOverlayHeight", typeof(double)),
            ("LootFilterManipulationEnabled", typeof(bool)));
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(7);
        applied["LeagueName"].Should().Be("Settlers");
        applied["FullSetThreshold"].Should().Be(3);
        applied["SoundEnabled"].Should().Be(true);
        applied["SoundLevel"].Should().Be(0.75);
        ((float)applied["StashTabOverlayOpacity"]).Should().BeApproximately(0.9f, 0.001f);
        applied["StashTabOverlayHeight"].Should().Be(720.0);
        applied["LootFilterManipulationEnabled"].Should().Be(false);
    }

    [Fact]
    public void ApplyLegacyValues_GivenCustomSkipList_RespectsIt()
    {
        // Arrange — custom skip list that blocks LeagueName
        var legacyValues = new Dictionary<string, string>
        {
            { "LeagueName", "Standard" },
            { "SoundEnabled", "True" },
        };
        var properties = BuildProperties(
            ("LeagueName", typeof(string)),
            ("SoundEnabled", typeof(bool)));
        var customSkip = new HashSet<string>(StringComparer.Ordinal) { "LeagueName" };
        var applied = new Dictionary<string, object>();

        // Act
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, customSkip,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(1);
        applied.Should().ContainKey("SoundEnabled");
        applied.Should().NotContainKey("LeagueName");
    }

    #endregion

    #region FindBestLegacyConfig

    /// <summary>
    /// These tests use real temp directories to verify the filesystem search logic.
    /// Each test creates isolated directories and cleans them up afterward.
    /// </summary>
    private sealed class TempDirFixture : IDisposable
    {
        public string Root { get; }

        public TempDirFixture()
        {
            Root = Path.Combine(Path.GetTempPath(), "CRE_Test_" + Guid.NewGuid().ToString("N")[..8]);
            Directory.CreateDirectory(Root);
        }

        /// <summary>
        /// Creates a candidate directory with an optional user.config file
        /// under the legacy version folder.
        /// </summary>
        public string CreateCandidate(string dirName, bool withConfig = true, DateTime? writeTime = null)
        {
            var dirPath = Path.Combine(Root, dirName);
            var versionDir = Path.Combine(dirPath, SettingsConfiguration.LegacyVersion);
            Directory.CreateDirectory(versionDir);

            if (withConfig)
            {
                var configPath = Path.Combine(versionDir, "user.config");
                File.WriteAllText(configPath, "<configuration />");
                if (writeTime.HasValue)
                {
                    File.SetLastWriteTimeUtc(configPath, writeTime.Value);
                }
            }

            return dirPath;
        }

        public void Dispose()
        {
            try { Directory.Delete(Root, recursive: true); } catch { }
        }
    }

    [Fact]
    public void FindBestLegacyConfig_GivenSingleCandidate_ReturnsIt()
    {
        // Arrange
        using var fixture = new TempDirFixture();
        var dir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_abc123");

        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            new[] { dir }, currentSettingsDir: null);

        // Assert
        result.Should().NotBeNull();
        result.Should().EndWith("user.config");
        result.Should().Contain(SettingsConfiguration.LegacyVersion);
    }

    [Fact]
    public void FindBestLegacyConfig_GivenNoCandidates_ReturnsNull()
    {
        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            Array.Empty<string>(), currentSettingsDir: null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindBestLegacyConfig_GivenCandidateWithoutConfig_ReturnsNull()
    {
        // Arrange — directory exists but no user.config file inside
        using var fixture = new TempDirFixture();
        var dir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_abc123", withConfig: false);

        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            new[] { dir }, currentSettingsDir: null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindBestLegacyConfig_GivenCurrentSettingsDir_SkipsIt()
    {
        // Arrange — the only candidate IS the current settings dir
        using var fixture = new TempDirFixture();
        var dir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_current");

        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            new[] { dir }, currentSettingsDir: dir);

        // Assert — should skip it and return null
        result.Should().BeNull();
    }

    [Fact]
    public void FindBestLegacyConfig_GivenCurrentDirAndLegacyDir_ReturnsLegacyOnly()
    {
        // Arrange
        using var fixture = new TempDirFixture();
        var currentDir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_current");
        var legacyDir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_legacy");

        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            new[] { currentDir, legacyDir }, currentSettingsDir: currentDir);

        // Assert
        result.Should().NotBeNull();
        result.Should().StartWith(legacyDir);
    }

    [Fact]
    public void FindBestLegacyConfig_GivenMultipleCandidates_ReturnsMostRecent()
    {
        // Arrange — two legacy configs with different timestamps
        using var fixture = new TempDirFixture();
        var olderDir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_older",
            writeTime: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var newerDir = fixture.CreateCandidate("ChaosRecipeEnhancer_StrongName_newer",
            writeTime: new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc));

        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            new[] { olderDir, newerDir }, currentSettingsDir: null);

        // Assert — should pick the newer one
        result.Should().StartWith(newerDir);
    }

    [Fact]
    public void FindBestLegacyConfig_GivenCurrentSettingsDirCaseInsensitive_SkipsIt()
    {
        // Arrange — verify case-insensitive comparison for Windows paths
        using var fixture = new TempDirFixture();
        var dir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_test");
        var uppercaseDir = dir.ToUpperInvariant();

        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            new[] { dir }, currentSettingsDir: uppercaseDir);

        // Assert — should still skip it despite case difference
        result.Should().BeNull();
    }

    [Fact]
    public void FindBestLegacyConfig_GivenMixOfValidAndInvalid_ReturnsOnlyValid()
    {
        // Arrange — one valid candidate, one without config, one that's the current dir
        using var fixture = new TempDirFixture();
        var noConfigDir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_noconfig", withConfig: false);
        var currentDir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_current");
        var validDir = fixture.CreateCandidate("ChaosRecipeEnhancer_Url_valid");

        // Act
        var result = SettingsConfiguration.FindBestLegacyConfig(
            new[] { noConfigDir, currentDir, validDir }, currentSettingsDir: currentDir);

        // Assert — only the valid, non-current directory should be returned
        result.Should().NotBeNull();
        result.Should().StartWith(validDir);
    }

    #endregion

    #region SkipSettings Verification

    [Fact]
    public void SkipSettings_ContainsAllSensitiveSettings()
    {
        // Assert — verify the skip list contains exactly the expected entries.
        // If a new sensitive setting is added to the app, this test reminds
        // the developer to add it to the skip list too.
        SettingsConfiguration.SkipSettings.Should().Contain("PathOfExileApiAuthToken");
        SettingsConfiguration.SkipSettings.Should().Contain("PathOfExileApiAuthTokenExpiration");
        SettingsConfiguration.SkipSettings.Should().Contain("LegacyAuthSessionId");
        SettingsConfiguration.SkipSettings.Should().Contain("UpgradeSettingsAfterUpdate");
        SettingsConfiguration.SkipSettings.Should().Contain("LegacySettingsMigrated");
    }

    [Fact]
    public void SkipSettings_IsCaseSensitive()
    {
        // Assert — the skip list uses Ordinal comparison, so it should NOT match
        // differently-cased variants. This prevents accidental case-insensitive matches.
        SettingsConfiguration.SkipSettings.Should().NotContain("pathofexileapiauthtoken");
        SettingsConfiguration.SkipSettings.Should().NotContain("PATHOFEXILEAPIAUTHTOKEN");
    }

    #endregion

    #region Constants Verification

    [Fact]
    public void LegacyVersion_MatchesExpectedMigrationSource()
    {
        // Assert — if this changes, the migration logic needs to be updated too
        SettingsConfiguration.LegacyVersion.Should().Be("3.27.1000.0");
    }

    [Fact]
    public void SettingsFolderPrefix_MatchesAppName()
    {
        SettingsConfiguration.SettingsFolderPrefix.Should().Be("ChaosRecipeEnhancer");
    }

    #endregion

    #region End-to-End: Parse + Apply

    [Fact]
    public void ParseThenApply_GivenRealisticConfig_ImportsCorrectValues()
    {
        // Arrange — full round-trip: parse XML then apply to typed properties
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""LeagueName"" serializeAs=""String"">
                    <value>Settlers</value>
                  </setting>
                  <setting name=""FullSetThreshold"" serializeAs=""String"">
                    <value>3</value>
                  </setting>
                  <setting name=""SoundEnabled"" serializeAs=""String"">
                    <value>False</value>
                  </setting>
                  <setting name=""PathOfExileApiAuthToken"" serializeAs=""String"">
                    <value>should-be-skipped</value>
                  </setting>
                  <setting name=""LegacyAuthSessionId"" serializeAs=""String"">
                    <value>also-skipped</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        var properties = BuildProperties(
            ("LeagueName", typeof(string)),
            ("FullSetThreshold", typeof(int)),
            ("SoundEnabled", typeof(bool)),
            ("PathOfExileApiAuthToken", typeof(string)),
            ("LegacyAuthSessionId", typeof(string)));
        var applied = new Dictionary<string, object>();

        // Act
        var legacyValues = SettingsConfiguration.ParseUserConfigXml(doc);
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert
        count.Should().Be(3); // 5 in XML, 2 skipped
        applied["LeagueName"].Should().Be("Settlers");
        applied["FullSetThreshold"].Should().Be(3);
        applied["SoundEnabled"].Should().Be(false);
        applied.Should().NotContainKey("PathOfExileApiAuthToken");
        applied.Should().NotContainKey("LegacyAuthSessionId");
    }

    [Fact]
    public void ParseThenApply_GivenUnknownLegacySettings_IgnoresThem()
    {
        // Arrange — legacy config has settings that no longer exist in current schema
        var doc = XDocument.Parse(@"
            <configuration>
              <userSettings>
                <ChaosRecipeEnhancer.UI.Properties.Settings>
                  <setting name=""RemovedSetting"" serializeAs=""String"">
                    <value>old-value</value>
                  </setting>
                  <setting name=""AnotherRemovedOne"" serializeAs=""String"">
                    <value>42</value>
                  </setting>
                  <setting name=""LeagueName"" serializeAs=""String"">
                    <value>Standard</value>
                  </setting>
                </ChaosRecipeEnhancer.UI.Properties.Settings>
              </userSettings>
            </configuration>");

        // Current schema only has LeagueName
        var properties = BuildProperties(("LeagueName", typeof(string)));
        var applied = new Dictionary<string, object>();

        // Act
        var legacyValues = SettingsConfiguration.ParseUserConfigXml(doc);
        var count = SettingsConfiguration.ApplyLegacyValues(
            legacyValues, properties, SettingsConfiguration.SkipSettings,
            (name, value) => applied[name] = value);

        // Assert — only the setting that exists in both schemas gets imported
        count.Should().Be(1);
        applied.Should().ContainKey("LeagueName");
    }

    #endregion
}
