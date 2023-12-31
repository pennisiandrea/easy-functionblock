﻿using System.Linq.Expressions;
using System.Net.Mime;
using System.Xml;
using System.Resources;
using FilesContents;
using System.Text.RegularExpressions;

namespace EasyFunctionBlock
{
    class MyFunctionBlock
    {
        public const int MAX_FB_NAME_LEN = 17;
        private string ThisDirectory;
        private string? PackageName;
        private string FunctionBlockName;
        private int FunctionBlockType;
        private string FunctionBlockDirectory;

        private const string FB_NAME_KEYWORD = "__FBNAME__";
        private const string PKG_NAME_KEYWORD = "__PKGNAME__";
    
        public MyFunctionBlock(string thisDirectory, int functionBlockType, string functionBlockName)
        {
            ThisDirectory = thisDirectory;
            FunctionBlockName = functionBlockName;
            FunctionBlockType = functionBlockType;
            PackageName = Path.GetFileName(ThisDirectory);
            FunctionBlockDirectory = Path.Combine(ThisDirectory,FunctionBlockName);

            if (FunctionBlockType<1 || FunctionBlockType>2) throw new Exception("Exception: Unknown functionblock type.");
            if (!Directory.Exists(ThisDirectory)) throw new Exception("Exception: This directory not found.");
            if (PackageName == null) throw new Exception("Exception: Directory not found.");
        }
        public void Create()
        {
            if (FunctionBlockType == 1) CreateEnable();
            else if (FunctionBlockType == 2) CreateExecute();
        }
        private void CreateExecute()
        {
            string FileContent;
            string? FileName;

            // Create sub package for functionblock
            Directory.CreateDirectory(FunctionBlockDirectory);

            // Main file
            FileContent = ExecuteFB.MAIN_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).TrimStart();
            FileName = ExecuteFB.MAIN_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(FunctionBlockDirectory,FileName,FileContent);

            // Actions file
            FileContent = ExecuteFB.ACTIONS_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).TrimStart();
            FileName = ExecuteFB.ACTIONS_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(FunctionBlockDirectory,FileName,FileContent);

            // Package file
            FileContent = ExecuteFB.SUBPKG_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            FileName = ExecuteFB.SUBPKG_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(FunctionBlockDirectory,FileName,FileContent); 

            // IEC file
            FileContent = ExecuteFB.IEC_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).TrimStart();    
            FileName = Directory.GetFiles(ThisDirectory,ExecuteFB.IEC_FILE_NAME).FirstOrDefault();
            if (FileName == null)
            {
                FileName = ExecuteFB.IEC_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).Replace("*","lby");
                CreateFile(ThisDirectory,FileName,FileContent);
            } 
            else
            {           
                if (!HasIECFilePackages(FileName))  ConvertIECFromFilesToObjects(FileName);
                MergeIECFiles(FileName,FileContent);
            }

            // Types file
            FileContent = ExecuteFB.TYPES_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            FileName = ExecuteFB.TYPES_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(ThisDirectory,FileName,FileContent);
            
            // Function file
            FileContent = ExecuteFB.FUNCTION_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            FileName = ExecuteFB.FUNCTION_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            if (File.Exists(FileName)) MergeFUNFiles(FileName, "\n" + FileContent);
            else CreateFile(ThisDirectory,FileName,FileContent);

        }
        private void CreateEnable()
        {
            string FileContent;
            string? FileName;

            // Create sub package for functionblock
            Directory.CreateDirectory(FunctionBlockDirectory);

            // Main file
            FileContent = EnableFB.MAIN_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).TrimStart();
            FileName = EnableFB.MAIN_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(FunctionBlockDirectory,FileName,FileContent);

            // Actions file
            FileContent = EnableFB.ACTIONS_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).TrimStart();
            FileName = EnableFB.ACTIONS_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(FunctionBlockDirectory,FileName,FileContent);

            // Package file
            FileContent = EnableFB.SUBPKG_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            FileName = EnableFB.SUBPKG_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(FunctionBlockDirectory,FileName,FileContent); 
            
            // IEC file
            FileContent = EnableFB.IEC_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).TrimStart();    
            FileName = Directory.GetFiles(ThisDirectory,EnableFB.IEC_FILE_NAME).FirstOrDefault();
            if (FileName == null)
            {
                FileName = EnableFB.IEC_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName).Replace("*","lby");
                CreateFile(ThisDirectory,FileName,FileContent);
            } 
            else
            {           
                if (!HasIECFilePackages(FileName)) ConvertIECFromFilesToObjects(FileName);
                MergeIECFiles(FileName,FileContent);
            }

            // Types file
            FileContent = EnableFB.TYPES_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            FileName = EnableFB.TYPES_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            CreateFile(ThisDirectory,FileName,FileContent);
            
            // Function file
            FileContent =EnableFB.FUNCTION_FILE_CONTENT.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            FileName = EnableFB.FUNCTION_FILE_NAME.Replace(PKG_NAME_KEYWORD,PackageName).Replace(FB_NAME_KEYWORD,FunctionBlockName);
            if (File.Exists(FileName)) MergeFUNFiles(FileName, "\n" + FileContent);
            else CreateFile(ThisDirectory,FileName,FileContent);           
        }
        private void CreateFile(string path, string name, string content)
        {
            string pathAndName = Path.Combine(path,name);

            var a = File.Create(pathAndName);
            a.Close();
            
            using (StreamWriter writer = new StreamWriter(pathAndName,true)) 
                writer.Write(content);
        }
        private void MergeFUNFiles(string destPath, string content)
        {            
            using (StreamWriter writer = new StreamWriter(destPath,true)) 
                writer.Write(content);
        }
        private void MergeIECFiles(string destPath, string content)
        {
            XmlDocument TemplateXmlFile = new XmlDocument();
            XmlDocument TemplateXmlFileNS = new XmlDocument();
            XmlDocument ThisXmlFile = new XmlDocument();
            XmlNode? ThisObjectsNode;
            XmlNode? TemplateObjectsNode;
            XmlNodeList? TemplateObjectNodes;

            ThisXmlFile.Load(destPath);
            ThisObjectsNode = ThisXmlFile.SelectSingleNode("//*[local-name()='Objects']");
            if (ThisObjectsNode == null) throw new Exception("Cannot find Objects node in This IEC file.");
            
            TemplateXmlFile.LoadXml(content);
            TemplateObjectsNode = TemplateXmlFile.SelectSingleNode("//*[local-name()='Objects']");
            if (TemplateObjectsNode == null) throw new Exception("wrong template IEC");

            TemplateXmlFileNS.LoadXml(content.Replace(TemplateObjectsNode.NamespaceURI,ThisObjectsNode.NamespaceURI));
            TemplateObjectNodes = TemplateXmlFileNS.SelectNodes("//*[local-name()='Object']");                     
            if (TemplateObjectNodes == null) throw new Exception("Cannot find Object nodes in template IEC file.");

            foreach(XmlNode node in TemplateObjectNodes)
            {
                ThisObjectsNode.AppendChild(ThisXmlFile.ImportNode(node.Clone(),true));   
            }

            // Check for duplicates
            foreach(XmlNode node1 in ThisObjectsNode)
            {
                var cnt = 0;
                foreach(XmlNode node2 in ThisObjectsNode)
                {
                    if (node1.InnerText == node2.InnerText) cnt = cnt + 1;
                    if (cnt>1) ThisObjectsNode.RemoveChild(node2);
                }
            }

            ThisXmlFile.Save(destPath);
        }
        private bool HasIECFilePackages(string Path)
        {
            XmlDocument ThisXmlFile = new XmlDocument();
            XmlNode? ThisFilesNode;

            if (!File.Exists(Path)) throw new Exception("Exception: IEC file not found!");
            ThisXmlFile.Load(Path);
            ThisFilesNode = ThisXmlFile.SelectSingleNode("//*[local-name()='Objects']");

            return ThisFilesNode != null;
        }
        private void ConvertIECFromFilesToObjects(string Path)
        {
            XmlDocument ThisXmlFile = new XmlDocument();
            XmlNode? ThisFilesNode;

            ThisXmlFile.Load(Path);
            ThisFilesNode = ThisXmlFile.SelectSingleNode("//*[local-name()='Files']");
            if (ThisFilesNode == null) throw new Exception("Cannot find Files node in This IEC file.");
            
            XmlNode NewObjectsNode = ThisXmlFile.CreateElement("Objects",ThisFilesNode.NamespaceURI);

            foreach(XmlNode node in ThisFilesNode){
                XmlNode NewObjectNode = ThisXmlFile.CreateElement("Object",ThisFilesNode.NamespaceURI);
                XmlAttribute AttributeType = ThisXmlFile.CreateAttribute("Type");
                AttributeType.Value = "File";

                if (node.Attributes != null && NewObjectNode != null)
                {
                    if (NewObjectNode.Attributes != null)
                    {
                        NewObjectNode.Attributes.Append(AttributeType);
                        foreach(XmlAttribute attribute in node.Attributes) 
                            NewObjectNode.Attributes.Append((XmlAttribute)attribute.Clone());                        
                    }
                    NewObjectNode.InnerText = node.InnerText;
                    NewObjectsNode.AppendChild(NewObjectNode);
                }
            }   

            if (ThisFilesNode.ParentNode == null)
            {
                ThisFilesNode.InsertBefore(NewObjectsNode,ThisFilesNode);
                ThisFilesNode.RemoveAll();
            }
            else
            {
                ThisFilesNode.ParentNode.InsertBefore(NewObjectsNode,ThisFilesNode);
                ThisFilesNode.ParentNode.RemoveChild(ThisFilesNode);
            }

            ThisXmlFile.Save(Path);
        }
    }

    class Program
    {
        static void Main(string[] argv)
        {
            // Get This folder
            string? ThisDirectory;
            if (argv.Count()>=1) 
            {  
                ThisDirectory = argv[1];
                if (ThisDirectory == null || !Directory.Exists(ThisDirectory)) throw new Exception("Exception: Cannot retrieve This directory.");
            }
            else
            {
                try
                {
                    ThisDirectory = Directory.GetCurrentDirectory();
                    if (ThisDirectory == null) throw new Exception("Exception: Cannot retrieve This directory.");         
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("Working directory: " + ThisDirectory);
            }

            // Get function block name
            string? FunctionBlockName;
            try
            {
                if (argv.Count()>=2) 
                {
                    if (argv[2].Length > MyFunctionBlock.MAX_FB_NAME_LEN) FunctionBlockName = argv[2];
                    else throw new Exception("Exception: FunctionBlock name too long. Max " + MyFunctionBlock.MAX_FB_NAME_LEN.ToString() + " chars!");                    
                }
                else
                { 
                    var InputOk = false;
                    do
                    {
                        Console.WriteLine("Write the functionblock name: ");
                        FunctionBlockName = Console.ReadLine();
                        if (FunctionBlockName == null) throw new Exception("Exception: Empty FunctionBlock name");
                        else if (FunctionBlockName.Length > MyFunctionBlock.MAX_FB_NAME_LEN ) Console.WriteLine("FunctionBlock name too long. Max " + MyFunctionBlock.MAX_FB_NAME_LEN.ToString() + " chars!");
                        else if (!Regex.IsMatch(FunctionBlockName,"^[a-zA-Z][a-zA-Z0-9]*$")) Console.WriteLine("FunctionBlock name invalid!");
                        else InputOk = true;
                    } while (!InputOk);                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                return;
            }

            // Get function block type
            int FunctionBlockType;
            try 
            {
                if (argv.Count()>=3)
                {
                    FunctionBlockType = int.Parse(argv[3]);
                    if (FunctionBlockType<1 || FunctionBlockType>2) throw new Exception("Exception: Unknown functionblock type.");
                }
                else
                {
                    var InputOk = false;
                    string? answ;
                    do
                    {
                        Console.WriteLine("Select the functionblock type: 1)Enable 2)Executable");
                        answ = Console.ReadLine();
                        if (answ == null) throw new Exception("Exception: Empty functionblock type");
                        else if (!Regex.IsMatch(answ,"^[0-9]+$")) Console.WriteLine("FunctionBlock type invalid!"); 
                        else if (int.Parse(answ)<1 || int.Parse(answ)>2) Console.WriteLine("FunctionBlock type out of range!"); 
                        else InputOk = true;

                    } while (!InputOk);   
                    FunctionBlockType = int.Parse(answ);                   
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                return;
            }

            // Create MyFunctionBlock object
            MyFunctionBlock myFunctionBlock;            
            try
            {
                myFunctionBlock = new MyFunctionBlock(ThisDirectory,FunctionBlockType,FunctionBlockName);

                myFunctionBlock.Create();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Done. Bye Bye");
        }

    }

}