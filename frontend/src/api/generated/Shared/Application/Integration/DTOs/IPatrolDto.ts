//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

import { IPosition } from '../../../CommonTypes/Geo/IPosition';
import { PatrolStatusEnum } from '../../../CommonTypes/Patrol/PatrolStatusEnum';

export interface IPatrolDto
{
	id: string;
	patrolId: string;
	position: IPosition;
	status: PatrolStatusEnum;
}