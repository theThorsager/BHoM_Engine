﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Linq;
using System.Reflection;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace BH.Engine.Serialiser.Objects.MemberMapConventions
{
    /// <summary>
    /// Represents the object discriminator convention.
    /// </summary>
    public class BHoMObjectDiscriminatorConvention : IDiscriminatorConvention
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string ElementName { get { return "_t"; } }


        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            // the BsonReader is sitting at the value whose actual type needs to be found
            var bsonType = bsonReader.GetCurrentBsonType();

            if (bsonType == BsonType.Document)
            {
                var bookmark = bsonReader.GetBookmark();
                bsonReader.ReadStartDocument();
                var actualType = nominalType;
                if (bsonReader.FindElement(ElementName))
                {
                    var context = BsonDeserializationContext.CreateRoot(bsonReader);
                    var discriminator = BsonValueSerializer.Instance.Deserialize(context);
                    actualType = BsonSerializer.LookupActualType(nominalType, discriminator);
                }
                else
                    actualType = typeof(CustomObject);

                bsonReader.ReturnToBookmark(bookmark);
                return actualType;
            }

            return nominalType;
        }

        /***************************************************/

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            return TypeNameDiscriminator.GetDiscriminator(actualType);
        }

        /***************************************************/
    }
}