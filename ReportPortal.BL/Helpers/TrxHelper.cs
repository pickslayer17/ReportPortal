using Microsoft.IdentityModel.Tokens;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models.TrxModels;
using System.Xml;
using System.Xml.Serialization;

namespace ReportPortal.BL.Helpers
{
    public static class TrxHelper
    {
        public static List<UnitTestModel> GetTestsFromTrxXml(string xml, int runId = default)
        {
            if (HasAnyTests(xml))
            {
                var serializer = new XmlSerializer(typeof(TestRun));
                TestRun trxModel;
                using (var stream = GenerateStreamFromString(xml))
                {
                    trxModel = (TestRun)serializer.Deserialize(stream);
                }

                var unitTests = trxModel.TestEntries?.Select(te => new UnitTestModel { Id = te.testId }).ToList();
                if (!unitTests.IsNullOrEmpty())
                {
                    var malformedTests = new List<UnitTestModel>(); // always removed (can't be rendered)

                    // Collect test results. We keep every test (passed included) — the app
                    // stores the full run and lets the UI filter, it does not drop greens.
                    foreach (var test in unitTests)
                    {
                        var results = trxModel.Results?.FirstOrDefault(td => td.testId == test.Id);

                        // A test entry without a matching result is malformed; drop it.
                        if (results == null)
                        {
                            malformedTests.Add(test);
                            continue;
                        }

                        if (runId != default)
                        {
                            test.RunId = runId;
                        }

                        test.Outcome = results.outcome;

                        // Passed tests carry no error info; nothing else to extract.
                        if (test.Outcome == TrxTestOutcome.Passed)
                        {
                            continue;
                        }

                        var errorInfo = results.Output?.ErrorInfo;
                        test.Message = errorInfo?.Message;
                        test.StackTrace = errorInfo?.StackTrace;
                    }

                    // Collect test info
                    foreach (var test in unitTests)
                    {
                        var testDefinition = trxModel.TestDefinitions?.FirstOrDefault(td => td.id == test.Id);
                        var testMethod = testDefinition?.TestMethod;

                        // No definition / method => we can't build a name or folder path; drop it.
                        if (testMethod?.name == null)
                        {
                            malformedTests.Add(test);
                            continue;
                        }

                        test.Name = testMethod.name;
                        test.FullName = $"{testMethod.className}.{testMethod.name}";
                    }

                    malformedTests.ForEach(ttr => unitTests.Remove(ttr));

                    return unitTests;
                }
            }

            return new List<UnitTestModel>();
        }

        private static bool HasAnyTests(string stringXml)
        {
            if (stringXml is null || stringXml == string.Empty)
            {
                return false;
            }

            var xml = new XmlDocument();
            try
            {
                xml.LoadXml(stringXml);
            }
            catch (Exception)
            {
                return false;
            }

            // A file is worth parsing if it actually carries test results — regardless of the
            // run-level outcome. (A fully "Completed"/all-passed run still has tests to import.)
            var results = xml.SelectNodes("//*[name()='Results']/*[name()='UnitTestResult']");

            return results != null && results.Count > 0;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}
