INSERT INTO [DynamicAvatarLayers] ([CreatedAt], [CreatedBy], [Name]) VALUES (GETDATE(), 1, 'Legs');
INSERT INTO [DynamicAvatarLayers] ([CreatedAt], [CreatedBy], [Name]) VALUES (GETDATE(), 1, 'Torso');
INSERT INTO [DynamicAvatarLayers] ([CreatedAt], [CreatedBy], [Name]) VALUES (GETDATE(), 1, 'Head');


INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 1, 1, 'Legs 1');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 1, 2, 'Legs 2');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 1, 3, 'Legs 3');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 1, 4, 'Legs 4');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 1, 5, 'Legs 5');

INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 1, 'Torso 1');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 2, 'Torso 2');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 3, 'Torso 3');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 4, 'Torso 4');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 5, 'Torso 5');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 6, 'Torso 6');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 7, 'Torso 7');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 2, 8, 'Torso 8');

INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 1, 'Head 1');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 2, 'Head 2');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 3, 'Head 3');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 4, 'Head 4');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 5, 'Head 5');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 6, 'Head 6');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 7, 'Head 7');
INSERT INTO [DynamicAvatarElements] ([CreatedAt], [CreatedBy], [DynamicAvatarLayerId], [Id], [Name]) VALUES (GETDATE(), 1, 3, 8, 'Head 8');

SELECT * FROM [DynamicAvatarLayers]
SELECT * FROM [DynamicAvatarElements]	