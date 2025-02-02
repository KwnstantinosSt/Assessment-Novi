// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Gateway.Models;

using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot(ElementName = "Envelope", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
public class EcbEnvelope
{
    [XmlElement(ElementName = "subject", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public string Subject { get; set; }

    [XmlElement(ElementName = "Sender", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public EcbSender Sender { get; set; }

    [XmlElement(ElementName = "Cube", Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public EcbCubeWrapper CubeWrapper { get; set; }
}

public class EcbSender
{
    [XmlElement(ElementName = "name", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public string Name { get; set; }
}

public class EcbCubeWrapper
{
    [XmlElement(ElementName = "Cube", Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public EcbDateCube DateCube { get; set; }
}

public class EcbDateCube
{
    [XmlAttribute(AttributeName = "time")]
    public string Time { get; set; }

    [XmlElement(ElementName = "Cube", Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public List<EcbCurrencyRate> CurrencyRates { get; set; }
}

public class EcbCurrencyRate
{
    [XmlAttribute(AttributeName = "currency")]
    public string Currency { get; set; }

    [XmlAttribute(AttributeName = "rate")]
    public decimal Rate { get; set; }
}
