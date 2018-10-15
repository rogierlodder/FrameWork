using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RGO;

namespace FrameWorkUnitTests
{
    [TestClass]
    public class FWOTests
    {
        [TestMethod]
        public void EQP_double_CopyValues_test()
        {
            RGOBase.ClearAll();
            RGOBase T = null;
            EQP<double> E = new EQP<double>(1, 10100, "TestSubSys", "TestParname", 100.00, 10, 1000, Units.mBar, "TestDescription");
            EQP<double> E_new = new EQP<double>(1, 10101, "TestSubSys", "TestParname", 100.01, 9, 1001, Units.mBar, "TestDescription");
            RGOBase.AddClientID(123456);
            T = E;
            T.CopyValues(E_new);

            Assert.AreEqual(E_new.Value, (T as EQP<double>).Value);
            Assert.AreNotEqual(E_new.ID, (T as EQP<double>).ID);
        }

        [TestMethod]
        public void EQP_Serialization_Tests()
        {
            RGOBase.ClearAll();
            EQP<double> eqp1 = new EQP<double>(5, 10100, "TestSubSys", "testParName", 10.5, 5, 15, Units.mA, "Test description");
            EQP<double> eqp2 = new EQP<double>(5, 10101, "TestSubSys", "testParName", 11.5, 5, 15, Units.mA, "Test description");

            Assert.IsFalse(eqp1.Value == eqp2.Value);

            EQPHistData ehd = eqp1.GetEQPHistData();
            string serdata = ehd.GetSerializationString();

            EQPHistData ehd2 = EQPHistData.GetInstance(serdata);
            eqp2.SetValueFromString(ehd2.NewValue);

            Assert.IsTrue(eqp1.Value == eqp2.Value);
        }

        [TestMethod]
        public void SV_double_CopyValues_test()
        {
            RGOBase.ClearAll();
            RGOBase T = null;
            SV<double> sv1 = new SV<double>(1, 10100, "TestDescription");
            SV<double> sv2 = new SV<double>(1, 10101, "TestDescription");

            sv1.Value = 3.3;
            sv2.Value = 4.4;

            T = sv1;
            T.CopyValues(sv2);

            Assert.AreEqual(sv2.Value, (T as SV<double>).Value);
            Assert.AreNotEqual(sv2.ID, (T as SV<double>).ID);
        }

        [TestMethod]
        public void IO_CopyValues_Override_on_Server()
        {
            RGOBase.ClearAll();
            RGOBase T = null;
            AI analogIn1 = new AI(1, 10100, IOCategory.Real, "test description");
            AI analogIn2 = new AI(1, 10101, IOCategory.Real, "test description");

            analogIn1.RawValue = 5;
            analogIn1.OverRide = false;
            analogIn1.UseConvertFunction = true;


            analogIn2.RawValue = 10;
            analogIn2.OverRide = true;
            analogIn2.OverrideValue = 10;
            analogIn2.UseConvertFunction = false;

            T = analogIn1;

            Assert.AreEqual((T as AI).Value, analogIn1.Value);
            Assert.AreNotEqual(analogIn1.OverRide, analogIn2.OverRide);
            Assert.AreNotEqual(analogIn1.UseConvertFunction, analogIn2.UseConvertFunction);

            T.CopyValues(analogIn2);

            Assert.AreEqual((T as AI).Value, analogIn2.Value);
            Assert.AreEqual((T as AI).OverRide, analogIn2.OverRide);
            Assert.AreEqual((T as AI).UseConvertFunction, analogIn2.UseConvertFunction);

        }

        [TestMethod]
        public void IO_CopyValues_on_Client()
        {
            RGOBase.ClearAll();
            RGOBase.RunsOnServer = false;
            AI analogIn1 = new AI(1, 10100, IOCategory.Real, "test description");
            AI analogIn2 = new AI(1, 10101, IOCategory.Real, "test description");

            analogIn1.RawValue = 5;
            analogIn1.IsValid = true;
            analogIn1.IsOverRidden = true;

            analogIn2.RawValue = 10;
            analogIn2.IsValid = false;
            analogIn2.IsOverRidden = false;


            Assert.AreNotEqual(analogIn1.Value, analogIn2.Value);
            Assert.AreNotEqual(analogIn1.IsValid, analogIn2.IsValid);
            Assert.AreNotEqual(analogIn1.IsOverRidden, analogIn2.IsOverRidden);

            analogIn1.CopyValues(analogIn2);

            Assert.AreEqual(analogIn1.Value, analogIn2.Value);
            Assert.AreEqual(analogIn1.IsValid, analogIn2.IsValid);
            Assert.AreEqual(analogIn1.IsOverRidden, analogIn2.IsOverRidden);

            RGOBase.RunsOnServer = true;
        }

        [TestMethod]
        public void AI_test()
        {
            RGOBase.ClearAll();
            AI analogIn1 = new AI(1, 10100, IOCategory.Real, "test description");

            analogIn1.RawValue = 5;
            Assert.IsTrue(analogIn1.Value == 5);

            analogIn1.Convert = p => p * 2;
            analogIn1.RawValue = 5;
            Assert.IsTrue(analogIn1.Value == 10);

            analogIn1.OverrideValue = 20;
            analogIn1.OverRide = true;

            analogIn1.RawValue = 5;
            Assert.IsTrue(analogIn1.Value == 40);

            analogIn1.UseConvertFunction = false;
            analogIn1.RawValue = 5;
            Assert.IsTrue(analogIn1.Value == 20);

            Assert.ThrowsException<NotImplementedException>(() => analogIn1.Value = 1.0);
        }

        [TestMethod]
        public void AO_test()
        {
            RGOBase.ClearAll();
            AO aout1 = new AO(1, 10100, IOCategory.Real, "test description");

            aout1.Value = 5.0;
            Assert.IsTrue(aout1.RawValue == 5.0);

            aout1.Convert = p => p * 2.0;
            
            aout1.Value = 5.0;
            Assert.IsTrue(aout1.RawValue == 10.0);

            aout1.OverRide = true;
            aout1.OverrideValue = 20.0;
            aout1.UseConvertFunction = false;
            aout1.Value = 5.0;
            Assert.IsTrue(aout1.RawValue == 20.0);

            aout1.UseConvertFunction = true;
            aout1.Value = 5.0;
            Assert.IsTrue(aout1.RawValue == 40.0);

            Assert.ThrowsException<NotImplementedException>(() => aout1.RawValue = 1.0);
        }

        [TestMethod]
        public void DI_test()
        {
            RGOBase.ClearAll();
            DI di = new DI(6, 10100, IOCategory.Real, "test description");
            di.MustSerialize = false;

            Assert.IsTrue(di.Value == false);
            di.RawValue = true;

            Assert.IsTrue(di.MustSerialize == true);

            Assert.IsTrue(di.Value);
            di.OverRide = true;
            di.OverrideValue = true;
            di.RawValue = false;
            Assert.IsTrue(di.Value);
          

            Assert.ThrowsException<NotImplementedException>(() => di.Value = true);
        }

        [TestMethod]
        public void DO_test()
        {
            RGOBase.ClearAll();
            DO dout = new DO(6, 10100, IOCategory.Real, "test description");
            dout.MustSerialize = false;

            Assert.IsTrue(dout.RawValue == false);
            dout.Value = true;
            Assert.IsTrue(dout.RawValue);
            Assert.IsTrue(dout.MustSerialize == true);
            
            dout.OverRide = true;
            dout.OverrideValue = true;

            dout.Value = false;
            Assert.IsTrue(dout.RawValue);

            Assert.ThrowsException<NotImplementedException>(() => dout.RawValue = true);
        }

        [TestMethod]
        public void STP_Copyvalues_double()
        {
            RGOBase.ClearAll();
            RGOBase T = null;
            STP<double> setp1 = new STP<double>(5, 10100, "Test description");
            STP<double> setp2 = new STP<double>(5, 10101, "Test description");

            setp1.Value = 5;
            setp2.Value = 10;

            Assert.AreNotEqual(setp1.Value, setp2.Value);

            T = setp1;
            T.CopyValues(setp2);

            Assert.AreEqual((T as STP<double>).Value, setp2.Value);

        }

        [TestMethod]
        public void FWO_Array_test()
        {
            RGOBase.ClearAll();
            RGOBase T = null;
            ARRAY<double> farr1 = new ARRAY<double>(5, 10100, 10, "test description");
            ARRAY<double> farr2 = new ARRAY<double>(5, 10101, 10, "test description");

            for (int i = 0; i < 10; i++) farr1[i] = 1 / 10.0;
            for (int i = 0; i < 10; i++) farr2[i] = 1 / 20.0;
            for (int i = 0; i < 10; i++) Assert.AreNotEqual(farr1[i], farr2[i]);

            T = farr1;
            T.CopyValues(farr2);

            for (int i = 0; i < 10; i++) Assert.AreEqual((T as ARRAY<double>)[i], farr2[i]);
        }

        [TestMethod]
        public void FWO_GetValueAsString()
        {
            RGOBase.ClearAll();

            AI analogIn1 = new AI(1, 10100, IOCategory.Real, "test description");
            analogIn1.RawValue = 5.555;

            ARRAY<double> farr1 = new ARRAY<double>(5, 10101, 10, "test description");
            for (int i = 0; i < 10; i++) farr1[i] = 1 / 10.0;

            SV<double> sv1 = new SV<double>(1, 10102, "TestDescription");
            sv1.Value = 3.333;

            SV<string> sv2 = new SV<string>(1, 10103, "TestDescription");
            sv2.Value = "Hello, this is an SV";
            
            EQP<double> eqp1 = new EQP<double>(5, 10104, "TestSubSys", "testParName", 10.5, 5, 15, Units.mA, "Test description");

            RGOBase T = analogIn1;
            Assert.AreEqual(analogIn1.Value.ToString(), T.GetValueAsString(3));
            Assert.AreNotEqual(analogIn1.Value.ToString(), T.GetValueAsString(2));
            Assert.AreEqual(analogIn1.Value.ToString(), T.GetValueAsString());

            T = eqp1;
            Assert.AreEqual(eqp1.Value.ToString(), T.GetValueAsString(1));
            Assert.AreEqual(eqp1.Value.ToString(), T.GetValueAsString());

            T = sv1;
            Assert.AreEqual(sv1.Value.ToString(), T.GetValueAsString());

            T = sv2;
            Assert.AreEqual(sv2.Value.ToString(), T.GetValueAsString());
            Assert.AreNotEqual(sv2.Value.ToString(), T.GetValueAsString(3));

            T = farr1;
            Assert.AreNotEqual(farr1[3], T.GetValueAsString());

        }

        string Args { get; set; }
        public void Testdelegate(string args)
        {
            Args = args + "1";
        }
        public void Testdelegate2(string args)
        {
            Args = args + "2";
        }

        [TestMethod]
        public void GuiCMD_Copyvalues()
        {
            RGOBase.ClearAll();
            CMD cmd = new CMD(5, 10100, Testdelegate, "Test description");
            CMD cmd2 = new CMD(5, 10101, Testdelegate2, "Test description");

            cmd.Args = "X";
            cmd2.Args = "Y";

            RGOBase T = null;
            T = cmd;
            T.CopyValues(cmd2);

            Assert.AreEqual(Args, "Y1");
        }
    }
}
