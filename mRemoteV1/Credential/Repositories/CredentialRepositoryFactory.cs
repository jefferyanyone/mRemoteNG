﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using mRemoteNG.Config;
using mRemoteNG.Config.DataProviders;
using mRemoteNG.Config.Serializers;

namespace mRemoteNG.Credential.Repositories
{
    public class CredentialRepositoryFactory
    {
        private readonly ISerializer<IEnumerable<ICredentialRecord>, string> _serializer;
        private readonly IDeserializer<string, IEnumerable<ICredentialRecord>> _deserializer;

        public CredentialRepositoryFactory(ISerializer<IEnumerable<ICredentialRecord>, string> serializer, IDeserializer<string, IEnumerable<ICredentialRecord>> deserializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            if (deserializer == null)
                throw new ArgumentNullException(nameof(deserializer));

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public ICredentialRepository Build(XElement repositoryXElement)
        {
            var typeName = repositoryXElement.Attribute("TypeName")?.Value;
            if (typeName == "Xml")
                return BuildXmlRepository(repositoryXElement);
            throw new Exception("Could not build repository for the specified type");
        }

        private ICredentialRepository BuildXmlRepository(XElement repositoryXElement)
        {
            var stringId = repositoryXElement.Attribute("Id")?.Value;
            Guid id;
            Guid.TryParse(stringId, out id);
            if (id.Equals(Guid.Empty)) id = Guid.NewGuid();
            var config = new CredentialRepositoryConfig(id)
            {
                TypeName = repositoryXElement.Attribute("TypeName")?.Value,
                Title = repositoryXElement.Attribute("Title")?.Value,
                Source = repositoryXElement.Attribute("Source")?.Value
            };
            var dataProvider = new FileDataProvider(config.Source);
            var saver = new CredentialRecordSaver(dataProvider, _serializer);
            var loader = new CredentialRecordLoader(dataProvider, _deserializer);
            return new XmlCredentialRepository(config, saver, loader);
        }
    }
}