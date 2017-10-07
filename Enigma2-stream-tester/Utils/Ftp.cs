﻿using System;
using System.IO;
using System.Net;

namespace Enigma2_stream_tester.Utils
{
    public class Ftp
    {
        private const int BufferSize = 2048;
        private FtpWebRequest _ftpRequest;
        private FtpWebResponse _ftpResponse;
        private Stream _ftpStream;
        private readonly string _host;
        private readonly string _pass;
        private readonly string _user;

        /* Construct Object */

        public Ftp(string hostIP, string userName, string password)
        {
            _host = hostIP;
            _user = userName;
            _pass = password;
        }

        /* Download File */

        public void Download(string remoteFile, string localFile)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + remoteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                _ftpStream = _ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                var localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                var byteBuffer = new byte[BufferSize];
                if (_ftpStream != null)
                {
                    var bytesRead = _ftpStream.Read(byteBuffer, 0, BufferSize);
                    /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                    try
                    {
                        while (bytesRead > 0)
                        {
                            localFileStream.Write(byteBuffer, 0, bytesRead);
                            bytesRead = _ftpStream.Read(byteBuffer, 0, BufferSize);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                /* Resource Cleanup */
                localFileStream.Close();
                _ftpStream?.Close();
                _ftpResponse.Close();
                _ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /* Upload File */

        public void Upload(string remoteFile, string localFile)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + remoteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Establish Return Communication with the FTP Server */
                _ftpStream = _ftpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */
                var localFileStream = new FileStream(localFile, FileMode.Open);
                /* Buffer for the Downloaded Data */
                var byteBuffer = new byte[BufferSize];
                var bytesSent = localFileStream.Read(byteBuffer, 0, BufferSize);
                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesSent != 0)
                    {
                        _ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, BufferSize);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                /* Resource Cleanup */
                localFileStream.Close();
                _ftpStream.Close();
                _ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /* Delete File */

        public void Delete(string deleteFile)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + deleteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Resource Cleanup */
                _ftpResponse.Close();
                _ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /* Rename File */

        public void Rename(string currentFileNameAndPath, string newFileName)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + currentFileNameAndPath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                /* Rename the File */
                _ftpRequest.RenameTo = newFileName;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Resource Cleanup */
                _ftpResponse.Close();
                _ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /* Create a New Directory on the FTP Server */

        public void CreateDirectory(string newDirectory)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + newDirectory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Resource Cleanup */
                _ftpResponse.Close();
                _ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /* Get the Date/Time a File was Created */

        public string GetFileCreatedDateTime(string fileName)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + fileName);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                _ftpStream = _ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                if (_ftpStream != null)
                {
                    var ftpReader = new StreamReader(_ftpStream);
                    /* Store the Raw Response */
                    string fileInfo = null;
                    /* Read the Full Response Stream */
                    try
                    {
                        fileInfo = ftpReader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    _ftpStream.Close();
                    _ftpResponse.Close();
                    _ftpRequest = null;
                    /* Return File Created Date Time */
                    return fileInfo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /* Get the Size of a File */

        public string GetFileSize(string fileName)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + fileName);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                _ftpStream = _ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                if (_ftpStream != null)
                {
                    var ftpReader = new StreamReader(_ftpStream);
                    /* Store the Raw Response */
                    string fileInfo = null;
                    /* Read the Full Response Stream */
                    try
                    {
                        while (ftpReader.Peek() != -1) fileInfo = ftpReader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    _ftpStream.Close();
                    _ftpResponse.Close();
                    _ftpRequest = null;
                    /* Return File Size */
                    return fileInfo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /* List Directory Contents File/Folder Name Only */

        public string[] DirectoryListSimple(string directory)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + directory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                _ftpStream = _ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                if (_ftpStream != null)
                {
                    var ftpReader = new StreamReader(_ftpStream);
                    /* Store the Raw Response */
                    string directoryRaw = null;
                    /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                    try
                    {
                        while (ftpReader.Peek() != -1)
                            directoryRaw += ftpReader.ReadLine() + "|";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    _ftpStream.Close();
                    _ftpResponse.Close();
                    _ftpRequest = null;
                    /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                    try
                    {
                        if (directoryRaw != null)
                        {
                            var directoryList = directoryRaw.Split("|".ToCharArray());
                            return directoryList;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return new[] {""};
        }

        /* List Directory Contents in Detail (Name, Size, Created, etc.) */

        public string[] DirectoryListDetailed(string directory)
        {
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + "/" + directory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                _ftpStream = _ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                if (_ftpStream != null)
                {
                    var ftpReader = new StreamReader(_ftpStream);
                    /* Store the Raw Response */
                    string directoryRaw = null;
                    /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                    try
                    {
                        while (ftpReader.Peek() != -1) directoryRaw += ftpReader.ReadLine() + "|";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    _ftpStream.Close();
                    _ftpResponse.Close();
                    _ftpRequest = null;
                    /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                    try
                    {
                        if (directoryRaw != null)
                        {
                            var directoryList = directoryRaw.Split("|".ToCharArray());
                            return directoryList;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return new[] {""};
        }
    }
}