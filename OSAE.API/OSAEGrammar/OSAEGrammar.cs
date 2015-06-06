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
                dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE base_type='PERSON' ORDER BY object_name");
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

        public static SpeechRecognitionEngine Load_OSA_Grammar(SpeechRecognitionEngine oRecognizer)
        {
            Choices nounPrecedentChoices = new Choices(new string[] { "a", "an", "the" });
            DataSet dsResults = new DataSet();

            #region Build a List of all Objects

            List<string> objectFullList = new List<string>();
            //Get All objects
            dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object ORDER BY object_name");
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
            dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object ORDER BY object_name");
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
            dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE container=1 ORDER BY object_name");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) containerList.Add(grammer);
            }
            Choices containerChoices = new Choices(containerList.ToArray());

            #endregion

            #region Build a List of all Object Types

            List<string> objectTypeList = new List<string>();
            dsResults = OSAESql.RunSQL("SELECT object_type FROM osae_v_object_type WHERE base_type NOT IN ('CONTROL','SCREEN') ORDER BY object_type");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) objectTypeList.Add(grammer);
            }
            Choices objectTypeChoices = new Choices(objectTypeList.ToArray());

            #endregion

            #region [Object] [State]
            // Is OBJECT STATE?
            // Is [NP] OBJECT STATE?
            // OBJECT is STATE
            // [NP] OBJECT is STATE
            GrammarBuilder gbAreYouState = new GrammarBuilder("Are");
            SemanticResultKey srk = new SemanticResultKey("PARAM1", "you");
            gbAreYouState.Append(srk);

            GrammarBuilder gbAmIState = new GrammarBuilder("Am I");
            srk = new SemanticResultKey("PARAM1", "I");
            gbAmIState.Append(srk);

            //builder.Append(objectChoices);

            foreach (string ot in objectTypeList)
            {
                List<string> objectList = new List<string>();
                List<string> stateList = new List<string>();

                GrammarBuilder gbIsObjectState = new GrammarBuilder("Is");
                GrammarBuilder gbIsNPObjectState = new GrammarBuilder("Is");
                gbIsNPObjectState.Append(nounPrecedentChoices);

                GrammarBuilder gbNPObjectIsState = new GrammarBuilder(nounPrecedentChoices);

                //Get All objects of the current Object Type
                dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE object_type='" + ot + "' ORDER BY object_name");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                    if (!string.IsNullOrEmpty(grammer)) objectList.Add(grammer);
                }
                if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                {
                    Choices objectChoices = new Choices(objectList.ToArray());
                    srk = new SemanticResultKey("PARAM1", objectChoices);

                    gbIsObjectState.Append(srk);
                    gbIsNPObjectState.Append(srk);
                    GrammarBuilder gbObjectIsState = new GrammarBuilder(srk);
                    gbNPObjectIsState.Append(srk);
                    gbObjectIsState.Append("is");
                    gbNPObjectIsState.Append("is");


                    //Now the the appropriate states                    
                    dsResults = OSAESql.RunSQL("SELECT DISTINCT(state_label) FROM osae_v_object_type_state WHERE object_type='" + ot + "'");
                    for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                    {
                        string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(grammer)) stateList.Add(grammer);
                    }
                    dsResults = OSAESql.RunSQL("SELECT DISTINCT(state_name) FROM osae_v_object_type_state WHERE object_type='" + ot + "' AND UPPER(state_name) != UPPER(state_label)");
                    for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                    {
                        string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(grammer)) stateList.Add(grammer);
                    }
                    if (stateList.Count > 0)
                    {
                        Choices stateChoices = new Choices(stateList.ToArray());
                        srk = new SemanticResultKey("PARAM2", stateChoices);
                        if (ot == "PERSON")
                        {
                            gbAmIState.Append(srk);
                            Grammar gAmIState = new Grammar(gbAmIState);
                            gAmIState.Name = "Is [OBJECT] [STATE]";
                            oRecognizer.LoadGrammar(gAmIState);
                        }
                        else if (ot == "SYSTEM")
                        {
                            gbAreYouState.Append(srk);
                            Grammar gAreYouState = new Grammar(gbAreYouState);
                            gAreYouState.Name = "Is [OBJECT] [STATE]";
                            oRecognizer.LoadGrammar(gAreYouState);
                        }

                        gbIsObjectState.Append(srk);
                        Grammar gIsObjectState = new Grammar(gbIsObjectState);
                        gIsObjectState.Name = "Is [OBJECT] [STATE]";
                        oRecognizer.LoadGrammar(gIsObjectState);

                        gbIsNPObjectState.Append(srk);
                        Grammar gIsNPObjectState = new Grammar(gbIsNPObjectState);
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

            #region What is [OBJECT]'s [PROPERTY]
            //What is OBJECT's PROPERTY
            //What is NP OBJECT's PROPERTY
            //What is my PROPERTY
            //What is your PROPERTY

            GrammarBuilder gbWhatIsMyProperty = new GrammarBuilder("What is");
            srk = new SemanticResultKey("PARAM1", "my");
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
                dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object WHERE object_type='" + ot + "' ORDER BY object_name");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                    if (!string.IsNullOrEmpty(grammer)) objectList.Add(grammer);
                }
                if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                {
                    Choices objectChoices = new Choices(objectList.ToArray());
                    srk = new SemanticResultKey("PARAM1", objectChoices);
                    gbWhatIsObjectProperty.Append(srk);
                    gbWhatIsNPObjectProperty.Append(srk);

                    //Now the the appropriate properties                    
                    dsResults = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_type_property WHERE object_type='" + ot + "' AND (property_datatype != 'Object Type' OR property_object_type != 'PERSON')");
                    for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                    {
                        string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(grammer)) propertyList.Add(grammer);
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
                dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object WHERE object_type='" + ot + "' ORDER BY object_name");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                    if (!string.IsNullOrEmpty(grammer)) objectList.Add(grammer);
                }
                if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                {
                    Choices objectChoices = new Choices(objectList.ToArray());
                    srk = new SemanticResultKey("PARAM1", objectChoices);
                    gbWhoIsObjectProperty.Append(srk);
                    gbWhoIsNPObjectProperty.Append(srk);

                    //Now the the appropriate properties                    
                    dsResults = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_type_property WHERE object_type='" + ot + "' AND property_datatype='Object Type' AND property_object_type='PERSON'");
                    for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                    {
                        string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(grammer)) propertyList.Add(grammer);
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
                dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object WHERE object_type='" + ot + "' ORDER BY object_name");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                    if (!string.IsNullOrEmpty(grammer)) objectList.Add(grammer);
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
                            DataSet dsPropObjectType = OSAESql.RunSQL("SELECT DISTINCT(object_name) FROM osae_v_object WHERE object_type='" + drType["property_object_type"].ToString() + "' ORDER BY object_name");
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

    /*
        public static string MatchPattern(string str, string sUser)
        {
            string ScriptParameter = "";
            sUser = sUser.ToUpper();
            try
            {
                str = str.ToUpper();
                str = str.TrimEnd('?', '.', '!') + " ";
                str = str.Replace(" 'S", "'S");
                str = str.Replace("YOUR ", "SYSTEM'S ");
                str = str.Replace("YOU ARE ", "SYSTEM IS ");
                str = str.Replace("ARE YOU ", "IS SYSTEM ");
                str = str.Replace("MY ", sUser + "'S ");
                str = str.Replace(" ME ", " " + sUser + " ");
                str = str.Replace("AM I ", "IS " + sUser + " ");
                str = str.Replace("I AM ", sUser + " IS ");

                DataSet dataset = new DataSet();
                //command.CommandText = "SELECT pattern FROM osae_v_pattern WHERE `match`=@Name";
                //command.Parameters.AddWithValue("@Name", str);
                dataset = OSAESql.RunSQL("SELECT pattern FROM osae_v_pattern_match WHERE UPPER(`match`)='" + str.Replace("'", "''") + "'");

                if (dataset.Tables[0].Rows.Count > 0)
                {

                    //Since we have a match, lets execute the scripts
                    OSAEScriptManager.RunPatternScript(dataset.Tables[0].Rows[0]["pattern"].ToString(), "", "SYSTEM");
                    return dataset.Tables[0].Rows[0]["pattern"].ToString();
                }
                else
                {
                    //Replace Words with place holders and retry the pattern match
                    //example  "Please turn the main light on" becomes "Please turn the [OBJECT] [STATE]"

                    //Step 1: Break the Input into an Array to Query the Words for DB matches


                    string[] words = str.Split(' ');

                    DataSet dsObjects = new DataSet();
                    foreach (String word in words)
                    {
                        dsObjects = OSAE.Common.ObjectNamesStartingWith(word.Replace("'S", ""));
                        foreach (DataRow dr in dsObjects.Tables[0].Rows)
                        {
                            if (str.IndexOf(dr["object_name"].ToString().ToUpper()) > -1)
                            //return "Found " + dr["object_name"].ToString();
                            {
                                str = str.Replace(dr["object_name"].ToString().ToUpper(), "[OBJECT]");
                                if (ScriptParameter.Length > 1)
                                {
                                    ScriptParameter = ScriptParameter + ",";
                                }
                                ScriptParameter += dr["object_name"].ToString();
                                //Determine if the Object is Possessive, which would be followed by a Property
                                if (str.ToUpper().IndexOf("[OBJECT]'S") > -1)
                                {
                                    //Here We have found our Possessive Object, so we need to look for an appropriate property afterwards
                                    //So we are going to retrieve a property list and compare it to the start of theremainder of the string

                                    DataSet dsProperties = new DataSet();
                                    dsProperties = OSAEObjectPropertyManager.ObjectPropertyListGet(dr["object_name"].ToString());
                                    foreach (DataRow drProperty in dsProperties.Tables[0].Rows)
                                    {
                                        //Here we need to break the string into words to avoid partial matches
                                        int objectStartLoc = str.ToUpper().IndexOf("[OBJECT]'S");
                                        string strNewSearch = str.Substring(objectStartLoc + 11);
                                        if (strNewSearch.ToUpper().IndexOf(drProperty["property_name"].ToString().ToUpper()) > -1)
                                        {
                                            str = str.Replace("[OBJECT]'S " + drProperty["property_name"].ToString().ToUpper(), "[OBJECT]'S [PROPERTY]");
                                            //str = str.Replace(drState["state_label"].ToString().ToUpper(), "[STATE]");
                                            ScriptParameter += "," + drProperty["property_name"].ToString();
                                        }
                                    }
                                }
                                string replacementString = "";
                                //Here We have found our Object, so we need to look for an appropriate Object Type afterwards
                                //So we are going to retrieve a object type list and compare it to the remainder of the string
                                DataSet dsObjectTypes = OSAESql.RunSQL("SELECT object_type FROM osae_v_object_type WHERE base_type NOT IN ('CONTROL','SCREEN') ORDER BY object_type");
                                foreach (DataRow drObjectTypes in dsObjectTypes.Tables[0].Rows)
                                {
                                    //Here we need to break the string into words to avoid partial matches

                                    string[] wordArray = str.Split(new Char[] { ' ' });
                                    foreach (string w in wordArray)
                                    {
                                        if (replacementString.Length > 1)
                                        {
                                            replacementString = replacementString + " ";
                                        }
                                        if (drObjectTypes["object_type"].ToString().ToUpper() == w)
                                        {
                                            replacementString = replacementString + "[OBJECT TYPE]";
                                            //str = str.Replace(drState["state_label"].ToString().ToUpper(), "[STATE]");
                                            ScriptParameter += "," + drObjectTypes["object_type"].ToString();
                                        }
                                        else
                                        {
                                            replacementString = replacementString + w;
                                        }
                                    }
                                }

                                //Here We have found our Object, so we need to look for an appropriate state afterwards
                                //So we are going to retrieve a state list and compare it to the remainder of the string
                                DataSet dsStates = new DataSet();
                                dsStates = OSAEObjectStateManager.ObjectStateListGet(dr["object_name"].ToString());
                                foreach (DataRow drState in dsStates.Tables[0].Rows)
                                {
                                    //Here we need to break the string into words to avoid partial matches
                                    string[] wordArray = str.Split(new Char[] { ' ' });
                                    foreach (string w in wordArray)
                                    {
                                        if (replacementString.Length > 1)
                                        {
                                            replacementString = replacementString + " ";
                                        }
                                        if (drState["state_label"].ToString().ToUpper() == w || drState["state_name"].ToString().ToUpper() == w)
                                        {
                                            replacementString = replacementString + "[STATE]";
                                            //str = str.Replace(drState["state_label"].ToString().ToUpper(), "[STATE]");
                                            ScriptParameter += "," + drState["state_name"].ToString();
                                        }
                                        else
                                        {
                                            replacementString = replacementString + w;
                                        }
                                    }
                                    //Now that we have replaced the Object,Object Type, and State, Lets check for a match again
                                    //DataSet dataset = new DataSet();
                                    //command.CommandText = "SELECT pattern FROM osae_v_pattern WHERE `match`=@Name";
                                    //command.Parameters.AddWithValue("@Name", str);
                                    //dataset = OSAESql.RunQuery(command);
                                    replacementString = replacementString.Replace(" 'S", "'S");
                                    dataset = OSAESql.RunSQL("SELECT pattern FROM osae_v_pattern_match WHERE `match`='" + replacementString.Replace("'", "''") + "'");
                                    if (dataset.Tables[0].Rows.Count > 0)
                                    {
                                        //return dataset.Tables[0].Rows[0]["pattern"].ToString();
                                        //Since we have a match, lets execute the scripts
                                        OSAEScriptManager.RunPatternScript(dataset.Tables[0].Rows[0]["pattern"].ToString(), ScriptParameter, "Jabber");
                                        return dataset.Tables[0].Rows[0]["pattern"].ToString();
                                    }
                                    //break;
                                }
                                //break;
                            }
                        }
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - MatchPattern error: " + ex.Message, true);
                return string.Empty;
            }
        }
*/

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

        public static SpeechRecognitionEngine Load_Property_Grammar(SpeechRecognitionEngine oRecognizer,string propDatatype,string propObjectType)
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
    }
}
