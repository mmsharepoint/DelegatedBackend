import * as React from 'react';
import styles from './ExecBackend.module.scss';
import { AadHttpClientFactory, AadHttpClient, AadTokenProviderFactory, HttpClientResponse } from '@microsoft/sp-http';
import { IExecBackendProps } from './IExecBackendProps';
import { DefaultButton } from 'office-ui-fabric-react/lib/Button';
import { TextField } from 'office-ui-fabric-react/lib/TextField';

export const ExecBackend: React.FunctionComponent<IExecBackendProps> = (props) => {
  const [firstTextFieldValue, setFirstTextFieldValue] = React.useState('');

  const onChangeFirstTextFieldValue = React.useCallback(
    (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
      if (!newValue || newValue.length <= 35) { // Check for invalid alias chars instead ...
        setFirstTextFieldValue(newValue || '');
      }
    },
    [],
  );

  const createGroup = async () => {
    const factory: AadHttpClientFactory = props.serviceScope.consume(AadHttpClientFactory.serviceKey);
    const client = await factory.getClient("https://mmoeller.onmicrosoft.com/bbb6d6d5-331e-4de9-b668-cb33bda2aba5");       
    const requestUrl = `http://localhost:7071/api/CreateGroup?groupName=${firstTextFieldValue}`;
    const result: any = await (await client.get(requestUrl, AadHttpClient.configurations.v1)).json();
    console.log(result);
    alert("Creation done!");
  };

  return (
    <div className={ styles.execBackend }>
      <div className={ styles.container }>
        <div className={ styles.row }>
          <div className={ styles.column }>
            <TextField label="Group name"
                        value={firstTextFieldValue}
                        onChange={onChangeFirstTextFieldValue}
                      />
          </div>
          <div className={ styles.column }>
            <DefaultButton text="Create" onClick={createGroup}  />
          </div>
        </div>
      </div>
    </div>
  );  
}
