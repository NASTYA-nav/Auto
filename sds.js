<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' >
    <entity name='cr34c_credit' >
        <attribute name='cr34c_creditid' />
        <attribute name='cr34c_name' />
        <attribute name='createdon' />
        <order attribute='cr34c_name' descending='false' />
        <link-entity name='cr34c_agreement' from='cr34c_creditid' to='cr34c_creditid' link-type='inner' alias='by' >
            <link-entity name='cr34c_auto' from='cr34c_autoid' to='cr34c_autoid' link-type='inner' alias='bz' >
                <filter type='and' >
                    <condition attribute='cr34c_brandid' operator='eq' uiname='new' uitype='cr34c_brand' value='{2DA9DAC7-3B6B-ED11-9560-000D3AB16D83}' />
                </filter>
                <link-entity name='cr34c_model' from='cr34c_modelid' to='cr34c_modelid' link-type='inner' alias='ca' >
                    <filter type='and' >
                        <condition attribute='cr34c_name' operator='not-null' />
                    </filter>
                </link-entity>
            </link-entity>
        </link-entity>
    </entity>
</fetch>