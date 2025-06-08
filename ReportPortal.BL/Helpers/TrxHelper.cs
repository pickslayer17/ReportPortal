using Microsoft.IdentityModel.Tokens;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models.TrxModels;
using System.Xml;
using System.Xml.Serialization;

namespace ReportPortal.BL.Helpers
{
    public static class TrxHelper
    {
        public static List<UnitTestModel> GetTestsFromTrxXml(string xml, bool isNeedToRemovePassed = true, int runId = default)
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
                    var testsToRemove = new List<UnitTestModel>();

                    // Collect test results
                    foreach (var test in unitTests)
                    {
                        var results = trxModel.Results.First(td => td.testId == test.Id);

                        if (runId != default)
                        {
                            test.RunId = runId;
                        }

                        test.Outcome = results.outcome;

                        // Verify if test Passed or not and add passed to removeList
                        if (test.Outcome == TrxTestOutcome.Passed)
                        {
                            testsToRemove.Add(test);
                            continue;
                        }

                        var output = results.Output;
                        var errorMessaage = output.ErrorInfo.Message;
                        var callStack = output.ErrorInfo.StackTrace;
                        test.Message = errorMessaage;
                        test.StackTrace = callStack;
                    }

                    if (isNeedToRemovePassed)
                    {
                        testsToRemove.ForEach(ttr => unitTests.Remove(ttr));
                    }

                    // Collect test info
                    foreach (var test in unitTests)
                    {
                        var testDefinitions = trxModel.TestDefinitions.First(td => td.id == test.Id);
                        var name = testDefinitions.TestMethod.name;
                        var className = testDefinitions.TestMethod.className;

                        test.Name = name;
                        test.FullName = $"{className}.{name}";
                    }

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

            var resultSummary = xml.SelectSingleNode("//*[name()='TestRun']/*[name()='ResultSummary']").Attributes["outcome"].Value;

            return resultSummary != "Completed";
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
