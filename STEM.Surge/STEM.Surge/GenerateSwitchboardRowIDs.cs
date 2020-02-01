/*
 * Copyright 2019 STEM Management
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

namespace STEM.Surge
{
    /// <summary>
    /// Utility class used to generate unique IDs for Switchboard rows and their expanded instances (DeploymentController rows)
    /// </summary>
    public static class GenerateSwitchboardRowIDs
    {
        /// <summary>
        /// Generate a DeploymentControllerID
        /// </summary>
        /// <param name="r">The target Switchboard row</param>
        /// <param name="path">A single path from an expandable SourceDirectory</param>
        /// <returns></returns>
        public static string GenerateDeploymentControllerID(SwitchboardConfig.FileSourcesRow r, string path)
        {
            if (r == null)
                throw new System.ArgumentNullException(nameof(r));

            if (System.String.IsNullOrEmpty(path))
                throw new System.ArgumentNullException(nameof(path));

            using (System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] b = sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(path.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + r.DirectoryFilter.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + r.FileFilter.ToUpper(System.Globalization.CultureInfo.CurrentCulture)));
                return System.BitConverter.ToInt32(b, 0).ToString(System.Globalization.CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Generate a SwitchboardRow ID
        /// </summary>
        /// <param name="r">The target Switchboard row</param>
        /// <returns>An ID to be used in other method calls relating to this Switchboard row</returns>
        public static string GenerateSwitchboardRowID(SwitchboardConfig.FileSourcesRow r)
        {
            if (r == null)
                throw new System.ArgumentNullException(nameof(r));

            using (System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] b = sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(r.SourceDirectory.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + r.DirectoryFilter.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + r.FileFilter.ToUpper(System.Globalization.CultureInfo.CurrentCulture)));
                return System.BitConverter.ToInt32(b, 0).ToString(System.Globalization.CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Generate a SwitchboardRow ID
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="directoryFilter"></param>
        /// <param name="fileFilter"></param>
        /// <returns>An ID to be used in other method calls relating to this Switchboard row</returns>
        public static string GenerateSwitchboardRowID(string sourceDirectory, string directoryFilter, string fileFilter)
        {
            if (System.String.IsNullOrEmpty(sourceDirectory))
                throw new System.ArgumentNullException(nameof(sourceDirectory));

            if (System.String.IsNullOrEmpty(directoryFilter))
                throw new System.ArgumentNullException(nameof(directoryFilter));

            if (System.String.IsNullOrEmpty(fileFilter))
                throw new System.ArgumentNullException(nameof(fileFilter));

            using (System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] b = sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(sourceDirectory.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + directoryFilter.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + fileFilter.ToUpper(System.Globalization.CultureInfo.CurrentCulture)));
                return System.BitConverter.ToInt32(b, 0).ToString(System.Globalization.CultureInfo.CurrentCulture);
            }
        }
    }
}
