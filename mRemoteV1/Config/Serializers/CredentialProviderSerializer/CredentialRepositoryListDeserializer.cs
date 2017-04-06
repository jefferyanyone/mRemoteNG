﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using mRemoteNG.Credential;
using mRemoteNG.Credential.Repositories;

namespace mRemoteNG.Config.Serializers.CredentialProviderSerializer
{
    public class CredentialRepositoryListDeserializer
    {
        private readonly ISerializer<IEnumerable<ICredentialRecord>, string> _serializer;
        private readonly IDeserializer<string, IEnumerable<ICredentialRecord>> _deserializer;

        public CredentialRepositoryListDeserializer(ISerializer<IEnumerable<ICredentialRecord>, string> serializer, IDeserializer<string, IEnumerable<ICredentialRecord>> deserializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            if (deserializer == null)
                throw new ArgumentNullException(nameof(deserializer));

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public IEnumerable<ICredentialRepository> Deserialize(string xml)
        {
            var xdoc = XDocument.Parse(xml);
            var repoEntries = xdoc.Descendants("CredentialRepository");
            return repoEntries.Select(new CredentialRepositoryFactory(_serializer, _deserializer).Build);
        }
    }
}