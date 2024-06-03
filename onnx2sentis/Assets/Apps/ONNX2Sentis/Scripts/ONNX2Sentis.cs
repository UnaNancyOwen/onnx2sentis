using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;
using Unity.Sentis.ONNX;
using System.IO;
using System;

namespace Converter
{
    public class ONNX2Sentis : MonoBehaviour
    {
        private void Start()
        {
#if !UNITY_SERVER
            return;
#endif

            var default_handler = Debug.unityLogger.logHandler;
            var custom_handler = new CustomLogHandler(default_handler);
            Debug.unityLogger.logHandler = custom_handler;

            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length <= 1)
                {
                    throw new ArgumentException("please specify onnx file path. (onnx2sentis ./model.onnx)");
                }

                var onnx_file = Path.GetFullPath(args[1]);
                Debug.Log($"convert from {onnx_file}");
                if (Path.GetExtension(onnx_file) != ".onnx")
                {
                    throw new ArgumentException("please specify onnx file path. (onnx2sentis ./model.onnx)");
                }

                var converter = new ONNXModelConverter(onnx_file);

                var current_directory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                var sentis_file = Path.Join(current_directory, Path.GetFileName(onnx_file).Replace(".onnx" ,".sentis"));
                Debug.Log($"convert to {sentis_file}");

                var model = converter.Convert();

                ModelWriter.Save(sentis_file, model);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey(false);
            Debug.unityLogger.logHandler = default_handler;
            Application.Quit();
        }
    }
}
