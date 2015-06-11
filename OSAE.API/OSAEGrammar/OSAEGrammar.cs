namespace OSAE
{
    using System;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;
    using System.Data;
    using System.Speech;
    using System.Speech.Recognition;

    public class OSAEGrammar
    {
        
        public static SpeechRecognitionEngine Load_User_Grammar(SpeechRecognitionEngine oRecognizer)
        {
            //User

            List<string> userList = new List<string>();
            DataSet dsResults = new DataSet();
            try
            {
                //Load all users
                dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object_list_full WHERE base_type='PERSON'");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    userList.Add(dsResults.Tables[0].Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {throw new Exception("API.Grammar User Grammar 1: " + ex.Message, ex);}

            Choices userChoices = new Choices(userList.ToArray());

            try
            {
                GrammarBuilder builder = new GrammarBuilder("This is");
                SemanticResultKey srk = new SemanticResultKey("PARAM1", userChoices);
                builder.Append(srk);
                Grammar gram = new Grammar(builder);
                gram.Name = "This is [OBJECT]";
                oRecognizer.LoadGrammar(gram);
            }
            catch (Exception ex)
            { throw new Exception("API.Grammar User Grammar 2: " + ex.Message, ex); }

            try
            {
                GrammarBuilder builder = new GrammarBuilder("I am");
                SemanticResultKey srk = new SemanticResultKey("PARAM1", userChoices);
                builder.Append(srk);
                Grammar gram = new Grammar(builder);
                gram.Name = "This is [OBJECT]";
                oRecognizer.LoadGrammar(gram);
            }
            catch (Exception ex)
            { throw new Exception("API.Grammar User Grammar 3: " + ex.Message, ex); }
            return oRecognizer;
        }

        public static SpeechRecognitionEngine Load_Direct_Grammar(SpeechRecognitionEngine oRecognizer)
        {
            List<string> grammerList = new List<string>();
            DataSet dsResults = new DataSet();
            Choices myChoices = new Choices();
            try
            {
                //Load all unique patterns with no place-holders into a single grammer, our main one.
                dsResults = OSAESql.RunSQL("SELECT `match` FROM osae_pattern_match WHERE UPPER(`match`) NOT LIKE '%[OBJECT]%' AND UPPER(`match`) NOT LIKE '%[STATE]%' AND UPPER(`match`) NOT LIKE '%[PRONOUN]%' ORDER BY `match`");
                //grammerList.Add(gWakePhrase);
                //grammerList.Add(gSleepPhrase);
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string sTemp = dsResults.Tables[0].Rows[i][0].ToString();

                    if (!string.IsNullOrEmpty(sTemp))
                    {
                        SemanticResultKey srk = new SemanticResultKey(sTemp, sTemp);
                        myChoices.Add(srk);
                    }
                }
            }
            catch (Exception ex)
            { throw new Exception("API.Grammar Direct Grammar 1: " + ex.Message, ex); }
            try
            {
                GrammarBuilder builder = new GrammarBuilder(myChoices);
                Grammar gram = new Grammar(builder);
                gram.Name = "Direct Match";
                oRecognizer.LoadGrammar(gram);
            }
            catch (Exception ex)
            { throw new Exception("API.Grammar Direct Grammar 2: " + ex.Message, ex); }

            return oRecognizer;
        }

        public static SpeechRecognitionEngine Load_Object_State_Grammar(SpeechRecognitionEngine oRecognizer)
        {
            DataSet dsResults = new DataSet();

            Choices nounPrecedentChoices = new Choices(new string[] { "a", "an", "the" });

            #region Build a List of all Object Types
            List<string> objectTypeList = new List<string>();
            dsResults = OSAESql.RunSQL("SELECT DISTINCT(object_type) FROM osae_v_object_list_full ORDER BY object_type");
            foreach (DataRow dr in dsResults.Tables[0].Rows)
            {
                objectTypeList.Add(dr[0].ToString());
            }
            Choices objectTypeChoices = new Choices(objectTypeList.ToArray());
            #endregion

            #region [Object] [State]
            // 1 Are you STATE
            // 2 Am I STATE
            // 3 Is OBJECT STATE?
            // 4 Is [NP] OBJECT STATE?

            // 5 You are STATE
            // 6 I am STATE
            // 7 OBJECT is STATE
            // 8 [NP] OBJECT is STATE
            GrammarBuilder gb1 = new GrammarBuilder("Are");
            GrammarBuilder gb5 = new GrammarBuilder();
            SemanticResultKey srk = new SemanticResultKey("PARAM1", "you");
            gb1.Append(srk);
            gb5.Append(srk);

            GrammarBuilder gb2 = new GrammarBuilder("Am I");
            GrammarBuilder gb6 = new GrammarBuilder();
            srk = new SemanticResultKey("PARAM1", "I");
            gb2.Append(srk);
            gb6.Append(srk);

            //builder.Append(objectChoices);

            foreach (string ot in objectTypeList)
            {
                List<string> stateList = new List<string>();
                GrammarBuilder gb3 = new GrammarBuilder("Is");
                GrammarBuilder gb4 = new GrammarBuilder("Is");
                gb4.Append(nounPrecedentChoices);

                GrammarBuilder gbNPObjectIsState = new GrammarBuilder(nounPrecedentChoices);

                //Get All objects of the current Object Type
                List<string> objectList = new List<string>();
                dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object_list_full WHERE object_type='" + ot + "'");
                foreach (DataRow dr in dsResults.Tables[0].Rows)
                {
                    objectList.Add(dr[0].ToString());
                }
                if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                {
                    Choices objectChoices = new Choices(objectList.ToArray());
                    srk = new SemanticResultKey("PARAM1", objectChoices);

                    gb3.Append(srk);
                    gb4.Append(srk);
                    GrammarBuilder gbObjectIsState = new GrammarBuilder(srk);
                    gbNPObjectIsState.Append(srk);
                    gbObjectIsState.Append("is");
                    gbNPObjectIsState.Append("is");

                    //Now the the appropriate states                    
                    dsResults = OSAESql.RunSQL("SELECT state_name FROM osae_v_object_type_state_list_full WHERE object_type='" + ot + "'");
                    foreach (DataRow dr in dsResults.Tables[0].Rows)
                    {
                        stateList.Add(dr[0].ToString());
                    }
                    if (stateList.Count > 0)
                    {
                        Choices stateChoices = new Choices(stateList.ToArray());
                        srk = new SemanticResultKey("PARAM2", stateChoices);
                        if (ot == "PERSON")
                        {
                            gb2.Append(srk);
                            Grammar gAmIState = new Grammar(gb2);
                            gAmIState.Name = "Is [OBJECT] [STATE]";
                            oRecognizer.LoadGrammar(gAmIState);

                        }
                        else if (ot == "SYSTEM")
                        {
                            gb1.Append(srk);
                            Grammar g1 = new Grammar(gb1);
                            g1.Name = "Is [OBJECT] [STATE]";
                            oRecognizer.LoadGrammar(g1);

                            gb5.Append(srk);
                            Grammar g5 = new Grammar(gb5);
                            g5.Name = "Is [OBJECT] [STATE]";
                            oRecognizer.LoadGrammar(g5);
                        }

                        gb3.Append(srk);
                        Grammar gIsObjectState = new Grammar(gb3);
                        gIsObjectState.Name = "Is [OBJECT] [STATE]";
                        oRecognizer.LoadGrammar(gIsObjectState);

                        gb4.Append(srk);
                        Grammar gIsNPObjectState = new Grammar(gb4);
                        gIsNPObjectState.Name = "[OBJECT] is [STATE]";
                        oRecognizer.LoadGrammar(gIsNPObjectState);

                        gbObjectIsState.Append(srk);
                        Grammar gObjectIsState = new Grammar(gbObjectIsState);
                        gObjectIsState.Name = "[OBJECT] is [STATE]";
                        oRecognizer.LoadGrammar(gObjectIsState);

                        gbNPObjectIsState.Append(srk);
                        Grammar gNPObjectIsState = new Grammar(gbNPObjectIsState);
                        gNPObjectIsState.Name = "[OBJECT] is [STATE]";
                        oRecognizer.LoadGrammar(gNPObjectIsState);
                    }
                }
            }
            #endregion
            return oRecognizer;
        }

        public static SpeechRecognitionEngine Load_OSA_Grammar(SpeechRecognitionEngine oRecognizer)
        {
            oRecognizer = Load_Object_State_Grammar(oRecognizer);

            Choices nounPrecedentChoices = new Choices(new string[] { "a", "an", "the" });
            DataSet dsResults = new DataSet();

            #region Build a List of all Objects

            List<string> objectFullList = new List<string>();
            //Get All objects
            dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object_list_full");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) objectFullList.Add(grammer);
            }
            Choices objectFullChoices = new Choices(objectFullList.ToArray());
            #endregion

            #region Build a List of all Possessive Objects

            List<string> objectPossessiveList = new List<string>();
            //Get All objects
            dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') AS object_name FROM osae_v_object_list_full");
            //dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object ORDER BY object_name");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) objectPossessiveList.Add(grammer);
            }
            Choices objectPossessiveChoices = new Choices(objectPossessiveList.ToArray());
            #endregion

            #region Build a List of all Containers

            List<string> containerList = new List<string>();
            //Get All containers
            dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object_list_full WHERE container=1");
            foreach (DataRow dr in dsResults.Tables[0].Rows)
            {
                containerList.Add(dr[0].ToString());
            }
            Choices containerChoices = new Choices(containerList.ToArray());
            #endregion

            #region Build a List of all Object Types
            List<string> objectTypeList = new List<string>();
            dsResults = OSAESql.RunSQL("SELECT DISTINCT(object_type) FROM osae_v_object_list_full ORDER BY object_type");
            foreach (DataRow dr in dsResults.Tables[0].Rows)
            {
                objectTypeList.Add(dr[0].ToString());
            }
            Choices objectTypeChoices = new Choices(objectTypeList.ToArray());
            #endregion

           
            #region What is [OBJECT]'s [PROPERTY]
            //What is OBJECT's PROPERTY
            //What is NP OBJECT's PROPERTY
            //What is my PROPERTY
            //What is your PROPERTY

            GrammarBuilder gbWhatIsMyProperty = new GrammarBuilder("What is");
            SemanticResultKey srk = new SemanticResultKey("PARAM1", "my");
            gbWhatIsMyProperty.Append(srk);
            GrammarBuilder gbWhatIsYourProperty = new GrammarBuilder("What is");
            srk = new SemanticResultKey("PARAM1", "your");
            gbWhatIsYourProperty.Append(srk);

            foreach (string ot in objectTypeList)
            {
                GrammarBuilder gbWhatIsObjectProperty = new GrammarBuilder("What is");
                GrammarBuilder gbWhatIsNPObjectProperty = new GrammarBuilder("What is");
                gbWhatIsNPObjectProperty.Append(nounPrecedentChoices);

                List<string> objectList = new List<string>();
                List<string> propertyList = new List<string>();

                //Get All objects of the current Object Type
                dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object_list_full WHERE object_type='" + ot + "'");
                foreach (DataRow dr in dsResults.Tables[0].Rows)
                {
                    objectList.Add(dr[0].ToString());
                }
                if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                {
                    Choices objectChoices = new Choices(objectList.ToArray());
                    srk = new SemanticResultKey("PARAM1", objectChoices);
                    gbWhatIsObjectProperty.Append(srk);
                    gbWhatIsNPObjectProperty.Append(srk);

                    //Now the the appropriate properties                    
                    dsResults = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_type_property WHERE object_type='" + ot + "' AND (property_datatype != 'Object Type' OR property_object_type != 'PERSON')");
                    foreach (DataRow dr in dsResults.Tables[0].Rows)
                    {
                        propertyList.Add(dr[0].ToString());
                    }
                    if (propertyList.Count > 0)
                    {
                        Choices propertyChoices = new Choices(propertyList.ToArray());
                        srk = new SemanticResultKey("PARAM2", propertyChoices);
                        if (ot == "PERSON")
                        {
                            gbWhatIsMyProperty.Append(srk);
                            Grammar gWhatIsMyProperty = new Grammar(gbWhatIsMyProperty);
                            gWhatIsMyProperty.Name = "What is [OBJECT] [PROPERTY]";
                            oRecognizer.LoadGrammar(gWhatIsMyProperty);
                        }
                        else if (ot == "SYSTEM")
                        {
                            gbWhatIsYourProperty.Append(srk);
                            Grammar gWhatIsYourProperty = new Grammar(gbWhatIsYourProperty);
                            gWhatIsYourProperty.Name = "What is [OBJECT] [PROPERTY]";
                            oRecognizer.LoadGrammar(gWhatIsYourProperty);
                        }
                        gbWhatIsObjectProperty.Append(srk);
                        Grammar gWhatIsObjectProperty = new Grammar(gbWhatIsObjectProperty);
                        gWhatIsObjectProperty.Name = "What is [OBJECT] [PROPERTY]";
                        oRecognizer.LoadGrammar(gWhatIsObjectProperty);

                        gbWhatIsNPObjectProperty.Append(srk);
                        Grammar gWhatIsNPObjectProperty = new Grammar(gbWhatIsNPObjectProperty);
                        gWhatIsNPObjectProperty.Name = "What is [OBJECT] [PROPERTY]";
                        oRecognizer.LoadGrammar(gWhatIsNPObjectProperty);

                    }
                }
            }
            #endregion

            #region Who is [OBJECT]'s [PROPERTY]
            //Who is OBJECT's PROPERTY
            //Who is NP OBJECT's PROPERTY
            //Who is my PROPERTY
            //Who is your PROPERTY

            GrammarBuilder gbWhoIsMyProperty = new GrammarBuilder("Who is");
            srk = new SemanticResultKey("PARAM1", "my");
            gbWhoIsMyProperty.Append(srk);
            GrammarBuilder gbWhoIsYourProperty = new GrammarBuilder("Who is");
            srk = new SemanticResultKey("PARAM1", "your");
            gbWhoIsYourProperty.Append(srk);

            foreach (string ot in objectTypeList)
            {
                GrammarBuilder gbWhoIsObjectProperty = new GrammarBuilder("Who is");
                GrammarBuilder gbWhoIsNPObjectProperty = new GrammarBuilder("Who is");
                gbWhoIsNPObjectProperty.Append(nounPrecedentChoices);

                List<string> objectList = new List<string>();
                List<string> propertyList = new List<string>();

                //Get All objects of the current Object Type
                dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object_list_full WHERE object_type='" + ot + "'");
                foreach (DataRow dr in dsResults.Tables[0].Rows)
                {
                    objectList.Add(dr[0].ToString());
                }
                if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                {
                    Choices objectChoices = new Choices(objectList.ToArray());
                    srk = new SemanticResultKey("PARAM1", objectChoices);
                    gbWhoIsObjectProperty.Append(srk);
                    gbWhoIsNPObjectProperty.Append(srk);

                    //Now the the appropriate properties                    
                    dsResults = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_type_property WHERE object_type='" + ot + "' AND property_datatype='Object Type' AND property_object_type='PERSON'");
                    foreach (DataRow dr in dsResults.Tables[0].Rows)
                    {
                        propertyList.Add(dr[0].ToString());
                    }
                    if (propertyList.Count > 0)
                    {
                        Choices propertyChoices = new Choices(propertyList.ToArray());
                        srk = new SemanticResultKey("PARAM2", propertyChoices);
                        if (ot == "PERSON")
                        {
                            gbWhoIsMyProperty.Append(srk);
                            Grammar gWhoIsMyProperty = new Grammar(gbWhoIsMyProperty);
                            gWhoIsMyProperty.Name = "What is [OBJECT] [PROPERTY]";
                            oRecognizer.LoadGrammar(gWhoIsMyProperty);
                        }
                        else if (ot == "SYSTEM")
                        {
                            gbWhoIsYourProperty.Append(srk);
                            Grammar gWhoIsYourProperty = new Grammar(gbWhoIsYourProperty);
                            gWhoIsYourProperty.Name = "What is [OBJECT] [PROPERTY]";
                            oRecognizer.LoadGrammar(gWhoIsYourProperty);
                        }

                        gbWhoIsObjectProperty.Append(srk);
                        Grammar gWhoIsObjectProperty = new Grammar(gbWhoIsObjectProperty);
                        gWhoIsObjectProperty.Name = "What is [OBJECT] [PROPERTY]";
                        oRecognizer.LoadGrammar(gWhoIsObjectProperty);

                        gbWhoIsNPObjectProperty.Append(srk);
                        Grammar gWhoIsNPObjectProperty = new Grammar(gbWhoIsNPObjectProperty);
                        gWhoIsNPObjectProperty.Name = "What is [OBJECT] [PROPERTY]";
                        oRecognizer.LoadGrammar(gWhoIsNPObjectProperty);
                    }
                }
            }
            #endregion

            #region [OBJECT]'s [PROPERTY] is [VALUE]
            //OBJECT's PROPERTY is [VALUE]

            foreach (string ot in objectTypeList)
            {
                List<string> objectList = new List<string>();


                GrammarBuilder gbObjectPropertyIs = new GrammarBuilder();

                //Get All objects of the current Object Type
                dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object_list_full WHERE object_type='" + ot + "'");
                foreach (DataRow dr in dsResults.Tables[0].Rows)
                {
                    objectList.Add(dr[0].ToString());
                }
                if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                {
                    Choices objectChoices = new Choices(objectList.ToArray());
                    srk = new SemanticResultKey("PARAM1", objectChoices);
                    gbObjectPropertyIs.Append(srk);

                    //Now the the appropriate properties                    
                    DataSet dsPropType = OSAESql.RunSQL("SELECT DISTINCT(property_datatype),property_object_type FROM osae_v_object_type_property WHERE object_type='" + ot + "' ORDER BY property_datatype");
                    foreach (DataRow drType in dsPropType.Tables[0].Rows)
                    {
                        List<string> propertyList = new List<string>();
                        DataSet dsPropName = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_type_property WHERE object_type='" + ot + "' AND property_datatype='" + drType["property_datatype"].ToString() + "' ORDER BY property_datatype");
                        foreach (DataRow drName in dsPropName.Tables[0].Rows)
                        {
                            propertyList.Add(drName["property_name"].ToString());
                        }
                        Choices propertyChoices = new Choices(propertyList.ToArray());
                        if (drType["property_datatype"].ToString().ToUpper() == "STRING")
                        {
                            GrammarBuilder dictation = new GrammarBuilder();
                            dictation.AppendDictation();

                            srk = new SemanticResultKey("PARAM2", propertyChoices);
                            gbObjectPropertyIs.Append(srk);
                            gbObjectPropertyIs.Append("is");
                            gbObjectPropertyIs.Append(new SemanticResultKey("PARAM3", dictation));
                            Grammar gObjectPropertyIs = new Grammar(gbObjectPropertyIs);
                            gObjectPropertyIs.Name = "[OBJECT] [PROPERTY] is [VALUE]";
                            oRecognizer.LoadGrammar(gObjectPropertyIs);
                        }
                        else if (drType["property_datatype"].ToString().ToUpper() == "OBJECT")
                        {
                            srk = new SemanticResultKey("PARAM2", propertyChoices);
                            gbObjectPropertyIs.Append(srk);
                            gbObjectPropertyIs.Append("is");
                            gbObjectPropertyIs.Append(new SemanticResultKey("PARAM3", objectFullChoices));
                            Grammar gObjectPropertyIs = new Grammar(gbObjectPropertyIs);
                            gObjectPropertyIs.Name = "[OBJECT] [PROPERTY] is [VALUE]";
                            oRecognizer.LoadGrammar(gObjectPropertyIs);
                        }
                        else if (drType["property_datatype"].ToString().ToUpper() == "OBJECT TYPE")
                        {
                            List<string> propertyOTList = new List<string>();
                            DataSet dsPropObjectType = OSAESql.RunSQL("SELECT object_name FROM osae_v_object_list_full WHERE object_type='" + drType["property_object_type"].ToString() + "'");
                            foreach (DataRow drName in dsPropObjectType.Tables[0].Rows)
                            {
                                propertyOTList.Add(drName["object_name"].ToString());
                            }
                            Choices propertyOTChoices = new Choices(propertyOTList.ToArray());
                            srk = new SemanticResultKey("PARAM2", propertyChoices);
                            gbObjectPropertyIs.Append(srk);
                            gbObjectPropertyIs.Append("is");

                            gbObjectPropertyIs.Append(new SemanticResultKey("PARAM3", propertyOTChoices));
                            Grammar gObjectPropertyIs = new Grammar(gbObjectPropertyIs);
                            gObjectPropertyIs.Name = "[OBJECT] [PROPERTY] is [VALUE]";
                            oRecognizer.LoadGrammar(gObjectPropertyIs);
                        }
                    }
                }
            }
            #endregion

            #region [OBJECT] [CONTAINER]
            // OBJECT is in CONTAINER
            // np OBJECT is in CONTAINER
            // OBJECT is in np CONTAINER
            // np OBJECT is in np CONTAINER
            // I am in CONTAINER
            // I am in np CONTAINER
            // You are in CONTAINER
            // You are in np CONTAINER


            // is OBJECT in CONTAINER
            // is np OBJECT is in CONTAINER
            // is OBJECT in np CONTAINER
            // is np OBJECT in np CONTAINER
            // am I in CONTAINER
            // am I in NP CONTAINER
            // are you in CONTAINER
            // are you in np CONTAINER

            // OBJECT is in CONTAINER
            GrammarBuilder gb_GrammarBuilder = new GrammarBuilder();
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("is in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            Grammar g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // np OBJECT is in CONTAINER
            gb_GrammarBuilder = new GrammarBuilder(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("is in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // OBJECT is in np CONTAINER
            gb_GrammarBuilder = new GrammarBuilder();
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("is in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // np OBJECT is in np CONTAINER
            gb_GrammarBuilder = new GrammarBuilder(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("is in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // I am in CONTAINER
            gb_GrammarBuilder = new GrammarBuilder();
            srk = new SemanticResultKey("PARAM1", "I");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("am in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // I am in np CONTAINER
            gb_GrammarBuilder = new GrammarBuilder();
            srk = new SemanticResultKey("PARAM1", "I");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("am in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // You are in CONTAINER
            gb_GrammarBuilder = new GrammarBuilder();
            srk = new SemanticResultKey("PARAM1", "you");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("are in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // You are in np CONTAINER
            gb_GrammarBuilder = new GrammarBuilder();
            srk = new SemanticResultKey("PARAM1", "you");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("are in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // is OBJECT in CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("is");
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // is np OBJECT is in CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("is");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // is OBJECT in np CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("is");
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("is in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // is np OBJECT in np CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("is");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // am I in CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("Am");
            srk = new SemanticResultKey("PARAM1", "I");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // am I in NP CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("Am");
            srk = new SemanticResultKey("PARAM1", "I");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // are you in CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("Are");
            srk = new SemanticResultKey("PARAM1", "you");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("in");
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);

            // are you in np CONTAINER
            gb_GrammarBuilder = new GrammarBuilder("Are");
            srk = new SemanticResultKey("PARAM1", "you");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append("in");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", containerChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
            oRecognizer.LoadGrammar(g_Grammar);
            #endregion

            #region [OBJECT] [OBJECT TYPE]
            // is OBJECT np OBJECT TYPE
            // is np OBJECT np OBJECT TYPE
            // am I np OBJECT TYPE
            // are you np OBJECT TYPE

            // is OBJECT np OBJECT TYPE
            gb_GrammarBuilder = new GrammarBuilder("is");
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", objectTypeChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
            oRecognizer.LoadGrammar(g_Grammar);

            // is np OBJECT np OBJECT TYPE
            gb_GrammarBuilder = new GrammarBuilder("is");
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", objectTypeChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
            oRecognizer.LoadGrammar(g_Grammar);

            // am I np OBJECT TYPE
            gb_GrammarBuilder = new GrammarBuilder("am");
            srk = new SemanticResultKey("PARAM1", "I");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", objectTypeChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
            oRecognizer.LoadGrammar(g_Grammar);

            // are you np OBJECT TYPE
            gb_GrammarBuilder = new GrammarBuilder("are");
            srk = new SemanticResultKey("PARAM1", "you");
            gb_GrammarBuilder.Append(srk);
            gb_GrammarBuilder.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM2", objectTypeChoices);
            gb_GrammarBuilder.Append(srk);
            g_Grammar = new Grammar(gb_GrammarBuilder);
            g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
            oRecognizer.LoadGrammar(g_Grammar);
            #endregion

            #region Where/What is [OBJECT]
            //Where is OBJECT
            //Where is NP OBJECT
            //Where am I
            //Where are You

            //What is OBJECT
            //What is NP OBJECT
            //What am I
            //What are You

            //Where is OBJECT
            GrammarBuilder gb_Single = new GrammarBuilder("Where is");
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_Single.Append(srk);
            Grammar g_Single = new Grammar(gb_Single);
            g_Single.Name = "Where is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);

            //Where is NP OBJECT
            gb_Single = new GrammarBuilder("Where is");
            gb_Single.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "Where is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);

            //Where am I
            gb_Single = new GrammarBuilder("Where am");
            srk = new SemanticResultKey("PARAM1", "I");
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "Where is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);

            //Where are you
            gb_Single = new GrammarBuilder("Where are");
            srk = new SemanticResultKey("PARAM1", "you");
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "Where is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);


            //What is OBJECT
            gb_Single = new GrammarBuilder("What is");
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "What is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);

            //What is NP OBJECT
            gb_Single = new GrammarBuilder("What is");
            gb_Single.Append(nounPrecedentChoices);
            srk = new SemanticResultKey("PARAM1", objectFullChoices);
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "What is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);

            //What am I
            gb_Single = new GrammarBuilder("What am");
            srk = new SemanticResultKey("PARAM1", "I");
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "What is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);

            //What are you
            gb_Single = new GrammarBuilder("What are");
            srk = new SemanticResultKey("PARAM1", "you");
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "What is [OBJECT]";
            oRecognizer.LoadGrammar(g_Single);
            #endregion

            #region Who is [PRONOUN]
            //Who am I
            //Who are you

            //Who am I
            gb_Single = new GrammarBuilder("Who am");
            srk = new SemanticResultKey("PARAM1", "I");
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "Who is [PERSON]";
            oRecognizer.LoadGrammar(g_Single);

            //Who are you
            gb_Single = new GrammarBuilder("Who are");
            srk = new SemanticResultKey("PARAM1", "you");
            gb_Single.Append(srk);
            g_Single = new Grammar(gb_Single);
            g_Single.Name = "Who is [PERSON]";
            oRecognizer.LoadGrammar(g_Single);
            #endregion
          
            return oRecognizer;
        }

        public static SpeechRecognitionEngine Load_Property_Grammar(SpeechRecognitionEngine oRecognizer, string propDatatype, string propObjectType)
        {
            #region [OBJECT]'s [PROPERTY]
            //OBJECT's PROPERTY

            List<string> noneList = new List<string>();
            noneList.Add("None");
            noneList.Add("Unknown");
            noneList.Add("Ignore");
            noneList.Add("Nevermind");
            Choices noneChoices = new Choices(noneList.ToArray());


            GrammarBuilder gb_Single = new GrammarBuilder();
            GrammarBuilder dictation = new GrammarBuilder();
            dictation.AppendDictation();
            gb_Single.Append(new SemanticResultKey("ANSWER", dictation));
            Grammar g_Single = new Grammar(gb_Single);
            g_Single.Name = "ANSWER";
            oRecognizer.LoadGrammar(g_Single);

            #endregion
            return oRecognizer;
        }

        public static string SearchForMeaning(string str, string ScriptParameter, string sUser)
        {
            DataSet dataset = new DataSet();
            dataset = OSAESql.RunSQL("SELECT pattern FROM osae_v_pattern_match WHERE `match`='" + str.Replace("'", "''") + "'");
            if (dataset.Tables[0].Rows.Count > 0)
            {
                //Since we have a match, lets execute the scripts
                OSAEScriptManager.RunPatternScript(dataset.Tables[0].Rows[0]["pattern"].ToString(), ScriptParameter, sUser);
                return dataset.Tables[0].Rows[0]["pattern"].ToString();
            }
            else
            {
                return "Sorry!";
            }
        }

        public static string GetQuestion()
        {
            DataSet dataset = new DataSet();
            dataset = OSAESql.RunSQL("CALL osae_sp_ai_get_question;'");
            if (dataset.Tables[0].Rows.Count > 0)
            {
                return dataset.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

    }
}
