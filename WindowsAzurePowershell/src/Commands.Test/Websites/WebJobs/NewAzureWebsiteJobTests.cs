﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Commands.Test.Websites
{
    using System.Management.Automation;
    using Commands.Utilities.Websites;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Commands.Utilities.Websites.Services.WebJobs;
    using Microsoft.WindowsAzure.Commands.Websites.WebJobs;
    using Moq;
    using Utilities.Websites;
    using Microsoft.WindowsAzure.WebSitesExtensions.Models;

    [TestClass]
    public class NewAzureWebsiteJobTests : WebsitesTestBase
    {
        private const string websiteName = "website1";

        private const string slot = "staging";

        private Mock<IWebsitesClient> websitesClientMock;

        private NewAzureWebsiteJobCommand cmdlet; 

        private Mock<ICommandRuntime> commandRuntimeMock;

        [TestInitialize]
        public override void SetupTest()
        {
            websitesClientMock = new Mock<IWebsitesClient>();
            commandRuntimeMock = new Mock<ICommandRuntime>();
            cmdlet = new NewAzureWebsiteJobCommand()
            {
                CommandRuntime = commandRuntimeMock.Object,
                WebsitesClient = websitesClientMock.Object,
                Name = websiteName,
                Slot = slot
            };
        }

        [TestMethod]
        public void CreatesTriggeredWebJob()
        {
            // Setup
            string jobName = "myWebJob";
            string jobFile = "job.bat";
            WebJobType jobType = WebJobType.Triggered;
            PSWebJob output = new PSWebJob() { JobName = jobName, JobType = jobType };
            websitesClientMock.Setup(f => f.CreateWebJob(websiteName, slot, jobName, jobType, jobFile)).Returns(output);
            cmdlet.JobName = jobName;
            cmdlet.JobType = jobType;
            cmdlet.JobFile = jobFile;

            // Test
            cmdlet.ExecuteCmdlet();

            // Assert
            websitesClientMock.Verify(f => f.CreateWebJob(websiteName, slot, jobName, jobType, jobFile), Times.Once());
            commandRuntimeMock.Verify(f => f.WriteObject(output), Times.Once());
        }
    }
}
